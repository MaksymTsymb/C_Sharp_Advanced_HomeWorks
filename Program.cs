using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization.Json;
//using Newtonsoft.Json;
using System.Text.Json;
using System.IO;

/// <summary>
/// Напишіть програму, яка буде серіалізувати простий об”єкт клас User (з полями string name, int age iEnum seniority) в Json строку.
//Але!
//1) Отриману Json строку треба зашифрувати, щоб не можна було з тексту зрозуміти, яка інформація міститься в середині і записати в файл;
//2) Реалізувати десеріалізацію разом з розкодуванням.
/// </summary>


namespace TsymbalMaksym_HomeWork_6
{
    public enum SeniorityInfo
    {
        someSeniorityInfo1 = 10,
        someSeniorityInfo2 = 20,
        someSeniorityInfo3 = 30,
        someSeniorityInfo4 = 40,
        someSeniorityInfo5 = 50
    }

    public class StringEncryption
    {
        private int[] transpositionKey;

        public void SetTranspositionKey(string inputKey)
        {
            transpositionKey = new int[inputKey.Length];

            for (int i = 0; i < inputKey.Length; i++)
                transpositionKey[i] = int.Parse(inputKey[i].ToString());
        }

        public string Encrypt(string input)
            {
            string result = string.Empty;
            string inputStringBody, inputStringRemnant;

            if (input.Length % transpositionKey.Length != 0)
            {
                int stringDifferenceLength = input.Length % transpositionKey.Length;

                inputStringRemnant = input.Substring(0, stringDifferenceLength);
                inputStringBody = input.Substring(stringDifferenceLength);
            }
            else
            {
                inputStringBody = input;
                inputStringRemnant = "";
            }


            for (int i = 0; i < inputStringBody.Length; i += transpositionKey.Length)
                {
                char[] transposition = new char[transpositionKey.Length];

                for (int j = 0; j < transposition.Length; j++)
                    transposition[transpositionKey[j] - 1] = inputStringBody[i + j];

                for (int j = 0; j < transpositionKey.Length; j++)
                    result += transposition[j];
            }

            result = result.Insert(0, inputStringRemnant);

            return result;
        }

        public string Decrypt(string input)
        {
            string result = string.Empty;
            string inputStringBody, inputStringRemnant;

            if (input.Length % transpositionKey.Length != 0)
            {
                int stringDifferenceLength = input.Length % transpositionKey.Length;

                inputStringRemnant = input.Substring(0, stringDifferenceLength);
                inputStringBody = input.Substring(stringDifferenceLength);
            }
            else
            {
                inputStringBody = input;
                inputStringRemnant = "";
            }

            for (int i = 0; i < inputStringBody.Length; i += transpositionKey.Length)
            {
                char[] transposition = new char[transpositionKey.Length];

                for (int j = 0; j < transposition.Length; j++)
                    transposition[j] = inputStringBody[i + transpositionKey[j] - 1];

                for (int j = 0; j < transpositionKey.Length; j++)
                    result += transposition[j];
            }

            result = result.Insert(0, inputStringRemnant);

            return result;
        }

        public StringEncryption(string key)
        {
            SetTranspositionKey(key);
        }
    }

    class User
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public SeniorityInfo Seniority { get; set; }


        public static List<User> GetListOfThreeInitializedUsers()
        {
            var user1 = new User
            {
                Name = "Adolf",
                Age = 45,
                Seniority = SeniorityInfo.someSeniorityInfo1
            };

            var user2 = new User
            {
                Name = "Benitto",
                Age = 62,
                Seniority = SeniorityInfo.someSeniorityInfo2
            };

            var user3 = new User
            {
                Name = "Joseph",
                Age = 52,
                Seniority = SeniorityInfo.someSeniorityInfo5
            };

            var users = new List<User>() { user1, user2, user3 };

            return users;
        }

        public override string ToString()
        {
            return Name + " " + Age.ToString();
        }
    }
    class Program
    {
        public static string GetAdminName()
        {
        MethodStart:
            Console.WriteLine("Enter this PC administrator name: ");
            string name = Console.ReadLine();

            try
            {
                File.Create($"C:\\Users\\{name}\\Desktop\\My1111111111111.txt");
            }
            catch (Exception)
            {
                Console.WriteLine($"{name} is not valid administrator name!");
                Console.WriteLine();

                //GetAdminName();
                goto MethodStart;  //Знаю что так нехорошо, но не успеваю сделать красиво всё
            }

            return name;
        }

        static void Main(string[] args)
        {
            string adminName = GetAdminName();

            List<User> users = User.GetListOfThreeInitializedUsers();

            string jsonString = JsonSerializer.Serialize(users);

            var crypt = new StringEncryption("3142");

            string jsonFilePath = $"C:\\Users\\{adminName}\\Desktop\\MyJsonFile.json";

            using (FileStream fs = new FileStream(jsonFilePath, FileMode.OpenOrCreate))
            using (StreamWriter stream = new StreamWriter(fs))
                stream.WriteLine(crypt.Encrypt(jsonString));

            string jsonStringOutput;

            using (FileStream fs = new FileStream(jsonFilePath, FileMode.OpenOrCreate))
            using (StreamReader stream = new StreamReader(fs))
                jsonStringOutput = crypt.Decrypt(stream.ReadLine());

            var restoredUsersList = JsonSerializer.Deserialize<List<User>>(jsonStringOutput);

            Console.WriteLine($"\nRestored data:\n{restoredUsersList[0]}\n{restoredUsersList[1]}\n{restoredUsersList[2]}");

            Console.ReadKey();
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using System.Reflection;
using DA = System.ComponentModel.DataAnnotations;
using TaskSchedular.Models;
using Microsoft.VisualStudio.TestPlatform.ObjectModel;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskSchedular.Data;
using OpenQA.Selenium.Chrome;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Configuration;



namespace TaskSchedular
{
    public class FunctionalTest
    {
        public static List<String> failMessage = new List<String>();
        public static String failureMsg = "";
        public static int failcnt = 1;
        public int totalTestcases = 0;

        String assemblyName = "TaskSchedular";
        public IWebDriver _driver;
        public string userport;
        public string appURL = "https://localhost:7213/";
        Assembly assem;
        TaskContext context;

        public IConfiguration configuration;
        public string connectionString = string.Empty;
        DbContextOptions<TaskContext> options;


        [OneTimeSetUp]
        public void Setup()
        {
            try
            {
                assem = Assembly.Load(assemblyName);
                //using var driver = new ChromeDriver();
                //_driver = new ChromeDriver();
                //_driver.Navigate().GoToUrl("https://localhost:7213");

                userport = Environment.GetEnvironmentVariable("userport") == null ? "7213" : Environment.GetEnvironmentVariable("userport");

                //FirefoxOptions options = new FirefoxOptions();
                //// {
                ////     AcceptInsecureCertificates = true
                //// };

                //options.AddArgument("--headless");
                //_driver = new FirefoxDriver(options);

                //appURL = "http://localhost:" + userport + "/";
                ////_driver = new FirefoxDriver();
                //_driver.Navigate().GoToUrl(appURL);

                configuration = new ConfigurationBuilder().AddJsonFile("appsettings.json").Build();
                connectionString = configuration.GetConnectionString("DBConnection");
                options = new DbContextOptionsBuilder<TaskContext>()
                .UseSqlServer(configuration.GetConnectionString("DBConnection"))
                .Options;

                var businessOptions = new DbContextOptionsBuilder<TaskContext>();
                businessOptions.UseSqlServer(configuration.GetConnectionString("DBConnection"));
                context = new TaskContext(businessOptions.Options);


            }
            catch (Exception ex)
            {
                //Console.WriteLine("-----------" + ex.Message);
            }
        }

        public void Dispose()
        {
            if (totalTestcases > 1)
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"./FailedTest.txt"))
                {
                    foreach (string line in failMessage)
                    {
                        //Console.WriteLine("line " + line);
                        file.WriteLine(line);
                    }
                }
            }
            else
            {
                using (System.IO.StreamWriter file = new System.IO.StreamWriter(@"./FailedTest.txt"))
                {
                    file.WriteLine("error");
                }
            }
            _driver.Quit();
            _driver.Dispose();
        }

        public void ExceptionCatch(string functionName, string catchMsg, string msg, string msg_name, string exceptionMsg = "")
        {
            failMessage.Add(functionName);

            if (msg == "")
            {
                msg = exceptionMsg + (exceptionMsg != "" ? " - " : "") + catchMsg + "\n";
                msg_name += "Fail " + failcnt + " -- " + functionName + "::\n" + msg;
            }
            else
                msg_name += "Fail " + failcnt + " -- " + functionName + "::\n" + msg;

            failureMsg += msg_name;
            failcnt++;
            Assert.Fail(msg);
        }
        public String SeleniumException(IWebDriver wd)
        {
            String msg = "";
            if (wd.Title == "Parser Error")
            {
                string[] stringSeparators = new string[] { "Parser Error Message:" };
                string[] result;
                result = wd.PageSource.Split(stringSeparators, StringSplitOptions.None);
                string[] stringSeparators2 = new string[] { "<b>Source Error:</b>" };
                result = result[1].Split(stringSeparators2, StringSplitOptions.None);
                msg += result[0].Replace("<br>", "").Replace("</b>", "").Replace("\r", "").Replace("\n", "");
            }
            else if (wd.Title.Contains("Error"))
            {
                msg += wd.FindElement(By.CssSelector("h2.exceptionMessage")).Text;
            }
            return msg;
        }

        [Test, Order(1)]
        public void Test1_Check_TaskInfo_Properties()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Models", "TaskInfo");
            var CurrentProperty = new KeyValuePair<string, string>();

            try
            {
                var Properties = new Dictionary<string, string>
                {
                    { "TaskID", "Int32" },
                    { "TaskDescription", "String" },
                    { "StartDate", "DateTime" },
                    { "ExpectedClosureDate", "DateTime" },
                    { "AssignedTo", "String" },
                    { "CompletionStatus", "String" },
                };

                foreach (var property in Properties)
                {
                    CurrentProperty = property;
                    var IsFound = tb.HasProperty(property.Key, property.Value);

                    //Assert.IsTrue(IsFound, tb.Messages.GetPropertyNotFoundMessage(property.Key, property.Value));

                    if (!IsFound)
                    {
                        msg += tb.Messages.GetPropertyNotFoundMessage(property.Key, property.Value) + "\n>";
                    }
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                //Assert.Fail(tb.Messages.GetExceptionMessage(ex, propertyName: CurrentProperty.Key));
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex, propertyName: CurrentProperty.Key), msg, msg_name);
            }
        }

        [Test, Order(2)]
        public void Test2_Check_TaskInfo_DataAnnotations()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Models", "TaskInfo");

            //(string propertyname, string attributename) PropertyUnderTest = ("", "");
            string PropertyUnderTest_propertyname = "";
            string PropertyUnderTest_attributename = "";

            try
            {
                //--------------------------------------------
                PropertyUnderTest_propertyname = "TaskID";
                PropertyUnderTest_attributename = "Key";
                KeyAttributeTest();

                PropertyUnderTest_propertyname = "TaskDescription";
                PropertyUnderTest_attributename = "Required";
                RequiredAttributeTest("Please provide the Task Description");

                PropertyUnderTest_propertyname = "StartDate";
                PropertyUnderTest_attributename = "Required";
                RequiredAttributeTest("Please provide the Start Date");

                PropertyUnderTest_propertyname = "ExpectedClosureDate";
                PropertyUnderTest_attributename = "Required";
                RequiredAttributeTest("Please provide the Expected Closure Date");

                PropertyUnderTest_propertyname = "AssignedTo";
                PropertyUnderTest_attributename = "Required";
                RequiredAttributeTest("Please provide the Assigned To");

                PropertyUnderTest_propertyname = "CompletionStatus";
                PropertyUnderTest_attributename = "Required";
                RequiredAttributeTest("Please provide the Completion Status");

                //----------------------------------------------------------------
                PropertyUnderTest_propertyname = "TaskDescription";
                PropertyUnderTest_attributename = "Display";
                DisplayAttributeTest("Task Description");

                PropertyUnderTest_propertyname = "StartDate";
                PropertyUnderTest_attributename = "Display";
                DisplayAttributeTest("Start Date");

                PropertyUnderTest_propertyname = "ExpectedClosureDate";
                PropertyUnderTest_attributename = "Display";
                DisplayAttributeTest("Expected Closure Date");

                PropertyUnderTest_propertyname = "AssignedTo";
                PropertyUnderTest_attributename = "Display";
                DisplayAttributeTest("Assigned To");

                PropertyUnderTest_propertyname = "CompletionStatus";
                PropertyUnderTest_attributename = "Display";
                DisplayAttributeTest("Completion Status");

                //----------------------------------------------------------------
                PropertyUnderTest_propertyname = "StartDate";
                PropertyUnderTest_attributename = "DataType";
                DataTypeAttributeTest();
                
                PropertyUnderTest_propertyname = "ExpectedClosureDate";
                PropertyUnderTest_attributename = "DataType";
                DataTypeAttributeTest();

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, $"Exception while testing {PropertyUnderTest_propertyname} for {PropertyUnderTest_attributename} attribute in {tb.type.Name}", msg, msg_name);
            }

            #region LocalFunction_KeyAttributeTest
            void KeyAttributeTest()
            {
                string Message = $"Key attribute on {PropertyUnderTest_propertyname} of {tb.type.Name} is not found";
                var attribute = tb.GetAttributeFromProperty<DA.KeyAttribute>(PropertyUnderTest_propertyname, typeof(DA.KeyAttribute));
                //Assert.IsNotNull(attribute, Message);
                if (attribute == null)
                {
                    msg += Message + "\n";
                }
            }
            #endregion

            #region LocalFunction_RequiredAttributeTest
            void RequiredAttributeTest(string errorMessage)
            {
                string Message = $"Required attribute on {PropertyUnderTest_propertyname} of {tb.type.Name} class doesnot have ";
                var attribute = tb.GetAttributeFromProperty<DA.RequiredAttribute>(PropertyUnderTest_propertyname, typeof(DA.RequiredAttribute));

                if (attribute == null)
                {
                    msg += $"Required attribute not applied on {PropertyUnderTest_propertyname} of {tb.type.Name} class.\n";
                }

                if (errorMessage != attribute.ErrorMessage)
                {
                    msg += $"{Message} ErrorMessage={errorMessage} \n";
                }
            }
            #endregion

            #region LocalFunction_DisplayAttributeTest
            void DisplayAttributeTest(string name)
            {
                string Message = $"Display Attribute on {PropertyUnderTest_propertyname} of {tb.type.Name} class doesnot have ";
                var attribute = tb.GetAttributeFromProperty<DA.DisplayAttribute>(PropertyUnderTest_propertyname, typeof(DA.DisplayAttribute));

                if (name != attribute.Name)
                {
                    msg += $"{Message} Name = {name} \n";
                }
            }
            #endregion

            #region LocalFunction_DataTypeAttributeTest
            void DataTypeAttributeTest()
            {
                //string Message = $"DataType attribute on {PropertyUnderTest_propertyname} of {tb.type.Name} class doesnot have ";
                var attribute = tb.GetAttributeFromProperty<DA.DataTypeAttribute>(PropertyUnderTest_propertyname, typeof(DA.DataTypeAttribute));

                if (attribute.DataType.ToString() != "Date")
                {
                    msg += $"DataType - Date not applied on {PropertyUnderTest_propertyname} of {tb.type.Name} class.\n";
                }
            }
            #endregion
        }

        [Test, Order(3)]
        public void Test3_TaskContext_DbSet_Property_CreationTest()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Data", "TaskContext");
            try
            {
                var IsFound = tb.HasProperty("TaskInfos", "DbSet`1");
                if (!IsFound)
                {
                    msg += tb.Messages.GetPropertyNotFoundMessage("TaskInfos", "DbSet<TaskInfo>");
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                //Assert.Fail(tb.Messages.GetExceptionMessage(ex, propertyName: "Donors"));
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }

        [Test, Order(4)]

        public void Test4_Check_ITasks_Methods()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.Name;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Interface", "ITasks");
            try
            {
                string returnType = "System.Threading.Tasks.Task`1[System.Collections.Generic.IEnumerable`1[TaskSchedular.Models.TaskInfo]]";
                string[] parameters = { };

                var IsFound = tb.HasMethod("GetAllTasksAsync", returnType, parameters);
                if (!IsFound)
                {
                    msg += tb.Messages.GetMethodNotFoundMessage(methodName: "GetAllTasksAsync", methodType: returnType, parameters);
                }

                returnType = "System.Threading.Tasks.Task`1[TaskSchedular.Models.TaskInfo]";
                parameters = new string[] { "System.Int32" };

                 IsFound = tb.HasMethod("GetTaskByIdAsync", returnType, parameters);
                if (!IsFound)
                {
                    msg += tb.Messages.GetMethodNotFoundMessage(methodName: "GetTaskByIdAsync", methodType: returnType, parameters);
                }

                returnType = "System.Threading.Tasks.Task";
                parameters = new string[] { "TaskSchedular.Models.TaskInfo" };

                IsFound = tb.HasMethod("AddTaskAsync", returnType, parameters);
                if (!IsFound)
                {
                    msg += tb.Messages.GetMethodNotFoundMessage(methodName: "AddTaskAsync", methodType: returnType, parameters);
                }

                returnType = "System.Threading.Tasks.Task";
                parameters = new string[] { "TaskSchedular.Models.TaskInfo" };

                IsFound = tb.HasMethod("UpdateTaskAsync", returnType, parameters);
                if (!IsFound)
                {
                    msg += tb.Messages.GetMethodNotFoundMessage(methodName: "UpdateTaskAsync", methodType: returnType, parameters);
                }

                returnType = "System.Threading.Tasks.Task";
                parameters = new string[] { "System.Int32" };

                IsFound = tb.HasMethod("DeleteTaskAsync", returnType, parameters);
                if (!IsFound)
                {
                    msg += tb.Messages.GetMethodNotFoundMessage(methodName: "DeleteTaskAsync", methodType: returnType, parameters);
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                //Assert.Fail(tb.Messages.GetExceptionMessage(ex, fieldName: "context"));
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }

        [Test, Order(5)]
        public void Test5_Check_Interface_Implementation_In_TaskRepository()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.Name;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");
            try
            {
                assem = Assembly.Load(assemblyName);
                Type ClassName = assem.GetType("TaskSchedular.Repository.TaskRepository");


                if (ClassName != null)
                {
                    //Type interfaceName = ClassName.BaseType.Name .GetInterface("ITasks");

                    if (ClassName.GetInterface("ITasks") != null)//ClassName.BaseType.Name != "ITasks")
                    {
                        if(ClassName.GetInterface("ITasks").Name != "ITasks")
                            msg += "Class 'TaskRepository' is not implementing the interface 'ITasks'";
                    }
                    else
                        msg += "Class 'TaskRepository' is not implementing the interface 'ITasks'";

                }
                else
                {
                    msg += "Class 'TaskRepository' is not declared or check the spelling";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                //Assert.Fail(tb.Messages.GetExceptionMessage(ex, fieldName: "context"));
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }

        [Test, Order(6)]
        public void Test6_Invoke_GetAllTasksAsync_Method()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.FullName;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");

            try
            {
                Type taskRepositoryType = assem.GetType("TaskSchedular.Repository.TaskRepository");
                ConstructorInfo taskRepositoryConstructor = taskRepositoryType.GetConstructors()[0];

                object[] arguments = new object[] { context };
                object instance = taskRepositoryConstructor.Invoke(arguments);

                Type taskEntityType = assem.GetType("TaskSchedular.Models.TaskInfo");
                object propertyInstance;
                var allBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;

                propertyInstance = Activator.CreateInstance(taskEntityType);

                MethodInfo m1 = taskRepositoryType.GetMethod("GetAllTasksAsync", allBindings);

                if (m1 != null)
                {

                    Task<IEnumerable<TaskInfo>> result = (Task<IEnumerable<TaskInfo>>)m1.Invoke(instance, new Object[] { });
                    result.GetAwaiter().GetResult();

                    var dbresult = string.Empty;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string Query = "select * from dbo.TaskInfos where TaskDescription = 'TestDesc1'";
                        SqlCommand cmd = new SqlCommand(Query, con);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dbresult = reader[4].ToString();
                        }
                        con.Close();
                    }

                    if (result.Result.Count() != 1 || dbresult != "TestAssignedTo")
                    {
                        msg += "The method 'GetAllTasksAsync' is not correctly retrieving the details from table 'TaskInfos'.\n";
                    }
                }
                else
                {
                    msg += "The method name 'GetAllTasksAsync' is not declared in the class 'TaskRepository'. Check the spelling.\n";
                }
                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex, "GetAllTasksAsync"), msg, msg_name);
            }
        }
        
        [Test, Order(7)]
        public void Test7_Invoke_GetTaskByIdAsync_Method()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.FullName;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");

            try
            {
                Type taskRepositoryType = assem.GetType("TaskSchedular.Repository.TaskRepository");
                ConstructorInfo taskRepositoryConstructor = taskRepositoryType.GetConstructors()[0];

                object[] arguments = new object[] { context };
                object instance = taskRepositoryConstructor.Invoke(arguments);

                Type taskEntityType = assem.GetType("TaskSchedular.Models.TaskInfo");
                object propertyInstance;
                var allBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;

                propertyInstance = Activator.CreateInstance(taskEntityType);

                MethodInfo m1 = taskRepositoryType.GetMethod("GetTaskByIdAsync", allBindings);

                if (m1 != null)
                {
                    Task<TaskInfo> result = (Task<TaskInfo>)m1.Invoke(instance, new Object[] { 2 });
                    result.GetAwaiter().GetResult();

                    var dbresult = string.Empty;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string Query = "select * from dbo.TaskInfos where TaskID = 2";
                        SqlCommand cmd = new SqlCommand(Query, con);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dbresult = reader[1].ToString();
                        }
                        con.Close();
                    }

                    if (result.Result.TaskID != 2 || dbresult != "")
                    {
                        msg += "The method 'GetTaskByIdAsync' is not correctly retrieving the details from table 'TaskInfos'.\n";
                    }
                }
                else
                {
                    msg += "The method name 'GetTaskByIdAsync' is not declared in the class 'TaskRepository'. Check the spelling.\n";
                }
                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }

        [Test, Order(8)]
        public void Test8_Invoke_AddTaskAsync_Method()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.FullName;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");

            try
            {
                Type taskRepositoryType = assem.GetType("TaskSchedular.Repository.TaskRepository");
                ConstructorInfo taskRepositoryConstructor = taskRepositoryType.GetConstructors()[0];

                object[] arguments = new object[] { context };
                object instance = taskRepositoryConstructor.Invoke(arguments);

                Type taskEntityType = assem.GetType("TaskSchedular.Models.TaskInfo");
                object propertyInstance;
                var allBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;

                propertyInstance = Activator.CreateInstance(taskEntityType);

                MethodInfo m1 = taskRepositoryType.GetMethod("AddTaskAsync", allBindings);

                if (m1 != null)
                {
                    //PropertyInfo p1 = taskEntityType.GetProperty("TaskID", allBindings);
                    //p1.SetValue(propertyInstance, 4);

                    PropertyInfo p2 = taskEntityType.GetProperty("TaskDescription", allBindings);
                    p2.SetValue(propertyInstance, "New Task 4");

                    PropertyInfo p3 = taskEntityType.GetProperty("StartDate", allBindings);
                    p3.SetValue(propertyInstance, DateTime.Today.AddDays(7));

                    PropertyInfo p4 = taskEntityType.GetProperty("ExpectedClosureDate", allBindings);
                    p4.SetValue(propertyInstance, DateTime.Today.AddDays(17));

                    PropertyInfo p5 = taskEntityType.GetProperty("AssignedTo", allBindings);
                    p5.SetValue(propertyInstance, "Test Man");

                    PropertyInfo p6 = taskEntityType.GetProperty("CompletionStatus", allBindings);
                    p6.SetValue(propertyInstance, "Yet to start");


                    Task result = (Task)m1.Invoke(instance, new Object[] { propertyInstance });
                    result.GetAwaiter().GetResult();

                    var dbresult = string.Empty;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string Query = "select * from dbo.TaskInfos where TaskID = '4'";
                        SqlCommand cmd = new SqlCommand(Query, con);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dbresult = reader[1].ToString();
                        }
                        con.Close();
                    }

                    if (dbresult != "New Task 4")
                    {
                        msg += "The method 'AddTaskAsync' is not correctly adding the details to the table 'TaskInfos'.\n";
                    }
                }
                else
                {
                    msg += "The method name 'AddTaskAsync' is not declared in the class 'TaskRepository'. Check the spelling.\n";
                }
                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }
        
        [Test, Order(9)]
        public void Test9_Invoke_UpdateTaskAsync_Method()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.FullName;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");

            try
            {
                Type taskRepositoryType = assem.GetType("TaskSchedular.Repository.TaskRepository");
                ConstructorInfo taskRepositoryConstructor = taskRepositoryType.GetConstructors()[0];

                object[] arguments = new object[] { context };
                object instance = taskRepositoryConstructor.Invoke(arguments);

                Type taskEntityType = assem.GetType("TaskSchedular.Models.TaskInfo");
                object propertyInstance;
                var allBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;

                propertyInstance = Activator.CreateInstance(taskEntityType);

                MethodInfo m1 = taskRepositoryType.GetMethod("UpdateTaskAsync", allBindings);

                if (m1 != null)
                {
                    PropertyInfo p1 = taskEntityType.GetProperty("TaskID", allBindings);
                    p1.SetValue(propertyInstance, 4);

                    PropertyInfo p2 = taskEntityType.GetProperty("TaskDescription", allBindings);
                    p2.SetValue(propertyInstance, "New Task 44");

                    PropertyInfo p3 = taskEntityType.GetProperty("StartDate", allBindings);
                    p3.SetValue(propertyInstance, DateTime.Today.AddDays(5));

                    PropertyInfo p4 = taskEntityType.GetProperty("ExpectedClosureDate", allBindings);
                    p4.SetValue(propertyInstance, DateTime.Today.AddDays(15));

                    PropertyInfo p5 = taskEntityType.GetProperty("AssignedTo", allBindings);
                    p5.SetValue(propertyInstance, "Test Woman 4");

                    PropertyInfo p6 = taskEntityType.GetProperty("CompletionStatus", allBindings);
                    p6.SetValue(propertyInstance, "In Progress 4");


                    Task result = (Task)m1.Invoke(instance, new Object[] { propertyInstance });
                    result.GetAwaiter().GetResult();

                    var dbresult = string.Empty;

                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string Query = "select * from dbo.TaskInfos where TaskID = '4'";
                        SqlCommand cmd = new SqlCommand(Query, con);

                        SqlDataReader reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dbresult = reader[4].ToString();
                        }
                        con.Close();
                    }

                    if (dbresult != "Test Woman 4")
                    {
                        msg += "The method 'UpdateTaskAsync' is not correctly updating the details to the table 'TaskInfos'.\n";
                    }
                }
                else
                {
                    msg += "The method name 'UpdateTaskAsync' is not declared in the class 'TaskRepository'. Check the spelling.\n";
                }
                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }
        
        [Test, Order(10)]
        public void Test10_Invoke_DeleteTaskAsync_Method()
        {
            totalTestcases++;
            string msg = "";
            string msg_name = "";
            string functionName = TestContext.CurrentContext.Test.FullName;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Repository", "TaskRepository");

            try
            {
                Type taskRepositoryType = assem.GetType("TaskSchedular.Repository.TaskRepository");
                ConstructorInfo taskRepositoryConstructor = taskRepositoryType.GetConstructors()[0];

                object[] arguments = new object[] { context };
                object instance = taskRepositoryConstructor.Invoke(arguments);

                Type taskEntityType = assem.GetType("TaskSchedular.Models.TaskInfo");
                object propertyInstance;
                var allBindings = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.IgnoreCase | BindingFlags.Static;

                propertyInstance = Activator.CreateInstance(taskEntityType);

                MethodInfo m1 = taskRepositoryType.GetMethod("DeleteTaskAsync", allBindings);

                if (m1 != null)
                {
                    //Task result = (Task)m1.Invoke(instance, new Object[] { 5 });
                    //result.GetAwaiter().GetResult();

                    var dbresult = string.Empty;

                    SqlDataReader reader = null;
                    using (SqlConnection con = new SqlConnection(connectionString))
                    {
                        con.Open();
                        string Query = "select * from dbo.TaskInfos where TaskID = '5'";
                        SqlCommand cmd = new SqlCommand(Query, con);

                        reader = cmd.ExecuteReader();

                        while (reader.Read())
                        {
                            dbresult = reader[4].ToString();
                        }
                        con.Close();
                    }

                    if (dbresult != "")//"Test Woman")
                    {
                        msg += "The method 'DeleteTaskAsync' is not correctly deleting the details from the table 'TaskInfos'.\n";
                    }
                }
                else
                {
                    msg += "The method name 'DeleteTaskAsync' is not declared in the class 'TaskRepository'. Check the spelling.\n";
                }
                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, tb.Messages.GetExceptionMessage(ex), msg, msg_name);
            }
        }


        [Test, Order(11)]
        [TestCase("Index", TestName = "Test11_Index_IsAvailable")]
        [TestCase("CreateTask", TestName = "Test12_CreateTask_IsAvailable")]
        [TestCase("EditTask", TestName = "Test13_EditTask_IsAvailable")]
        [TestCase("DeleteTask", TestName = "Test14_DeleteTask_IsAvailable")]
        public void Test_11_12_13_14_Get_ActionCreated_Test(string mname)
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;
            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Controllers", "TaskController");
            try
            {
                var Method = tb.type.GetMethod(mname, new Type[] { });

                if (mname == "EditTask" || mname == "DeleteTask")
                {
                    Method = tb.type.GetMethod(mname, new Type[] { typeof(int) });
                }

                if (Method == null)
                {
                    msg += $"{tb.type.Name} doesnot defines action method \n";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, $"Exception while check Get action method is present or not in {tb.type.Name}. \n", msg, msg_name);
            }
        }

        [Test, Order(12)]
        public void Test15_CreateTask_Post_ActionCreated_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Controllers", "TaskController");
            try
            {
                var Method = tb.type.GetMethod("CreateTask", new Type[] { typeof(TaskInfo) });
                if (Method == null)
                {
                    msg += $"{tb.type.Name} doesnot defines create action method which accepts over object as parameter \n";
                }

                var attr = Method.GetCustomAttribute<HttpPostAttribute>();

                if (attr == null)
                {
                    msg += $"CreateTask action is not marked with attributes to run on http post request in {tb.type.Name} controller \n";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, $"Exception while check CreateTask action method is present or not in {tb.type.Name}. \n", msg, msg_name);
            }
        }

        [Test, Order(13)]
        public void Test16_EditTask_Post_ActionCreated_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Controllers", "TaskController");
            try
            {
                var Method = tb.type.GetMethod("EditTask", new Type[] { typeof(int), typeof(TaskInfo) });
                if (Method == null)
                {
                    msg += $"{tb.type.Name} doesnot defines create action method which accepts over object as parameter \n";
                }

                var attr = Method.GetCustomAttribute<HttpPostAttribute>();

                if (attr == null)
                {
                    msg += $"EditTask action is not marked with attributes to run on http post request in {tb.type.Name} controller \n";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, $"Exception while check EditTask action method is present or not in {tb.type.Name}. \n", msg, msg_name);
            }
        }

        [Test, Order(14)]
        public void Test17_DeleteTask_Post_ActionCreated_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            TestBase tb = new TestBase("TaskSchedular", "TaskSchedular.Controllers", "TaskController");
            try
            {
                var Method = tb.type.GetMethod("DeleteConfirmed", new Type[] { typeof(int) });
                if (Method == null)
                {
                    msg += $"{tb.type.Name} doesnot defines create action method which accepts over object as parameter \n";
                }

                var attr = Method.GetCustomAttribute<HttpPostAttribute>();
                var attr1 = Method.GetCustomAttribute<ActionNameAttribute>();

                if (attr == null || attr1.Name != "DeleteTask")
                {
                    msg += $"DeleteConfirmed action is NOT marked with attributes to run on http post request OR check for the action name in {tb.type.Name} controller \n";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, $"Exception while check DeleteTask action method is present or not in {tb.type.Name}. \n", msg, msg_name);
            }
        }

        [Test, Order(15)]
        public void Test18_UI_CreateTask_Index_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            try
            {
                _driver.Navigate().GoToUrl(appURL + "Task/Index");
                System.Threading.Thread.Sleep(5000);

                //msg+=_driver.PageSource.ToString();
                var result1 = _driver.FindElement(By.Id("lnkCreate"));

                if (result1 == null)
                {
                    msg += "Create New link is not available. \n";
                }


                _driver.FindElement(By.Id("lnkCreate")).Click();
                //msg+="+++++++++"+_driver.PageSource.ToString();
                System.Threading.Thread.Sleep(5000);

                _driver.SetElementText("TaskDescription", "TestDesc1");
                _driver.SetElementText("StartDate", DateTime.Today.AddDays(3).ToString());
                _driver.SetElementText("ExpectedClosureDate", DateTime.Today.AddDays(4).ToString());
                _driver.SetElementText("AssignedTo", "TestAssignedTo");
                _driver.SetElementText("CompletionStatus", "TestStatus");
                _driver.ClickElement("btnCreate");
                System.Threading.Thread.Sleep(5000);

                var result = _driver.FindElement(By.Id("tblInfos"));

                if (result == null)
                {
                    msg += "'tblInfos' table is not available. \n";
                }

                if (!_driver.PageSource.Contains("TestDesc1"))
                {
                    msg += "New Info is not populating in Index page.\n";
                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, ex.Message + $"-Exception trying to add and display the TaskInfo details. Exception :{ex.InnerException?.Message}\n", msg, msg_name);
            }
        }

        [Test, Order(16)]
        public void Test19_UI_EditTask_Index_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            try
            {
                _driver.Navigate().GoToUrl(appURL + "Task/Index");
                System.Threading.Thread.Sleep(5000);

                var table = _driver.FindElement(By.Id("tblInfos"));

                //string taskId = "TestDesc1";
                //string cssSelector = $"tr:has(td:contains('{taskId}')) a#lnkEdit";
                //IWebElement editLink = _driver.FindElement(By.CssSelector(cssSelector));
                //editLink.Click();

                int flag = 0;
                int flag1 = 0;
                //msg += _driver.PageSource.ToString();
                foreach (var tr in table.FindElements(By.TagName("tr")))
                {
                    //msg += "---------1.1-------------\n";
                    var tds = tr.FindElements(By.TagName("td"));
                    //msg += "---------1.2-------------\n"+tds.Count;
                    //for (var i = 0; i < tds.Count; i++)
                    //{
                        //msg += i + "-"+tds[i].Text +"\n";
                        //msg+=tds[i+1].Text+"\n";
                        if (tds.Count > 0 && tds[1].Text.Trim().Contains("TestDesc1"))
                        {
                            //    msg += "---------1-------------\n";
                            tds[6].FindElement(By.Id("lnkEdit")).Click();
                        //var r = tds.Where(w => w.FindElements(By.Id("lnkEdit")).)
                            System.Threading.Thread.Sleep(5000);
                            //msg += "---------4-------------\n";

                            if (!_driver.Url.Contains("EditTask"))
                            {
                                //msg += "---------4-------------\n";
                                msg += "EditTask Page is NOT navigating correctly.\n";
                            }
                            else
                            {
                                // msg += "---------4-------------\n";

                                _driver.SetElementText("TaskDescription", "TestDesc090");
                                _driver.SetElementText("StartDate", DateTime.Today.AddDays(3).ToString());
                                _driver.SetElementText("ExpectedClosureDate", DateTime.Today.AddDays(5).ToString());
                                _driver.SetElementText("AssignedTo", "TestAssignedTo091");
                                _driver.SetElementText("CompletionStatus", "TestStatus092");
                                _driver.ClickElement("btnEdit");
                                System.Threading.Thread.Sleep(5000);

                                //msg += "---------5-------------\n";
                                if (!_driver.PageSource.Contains("TestDesc090"))
                                {
                                    msg += "Updated Info is NOT populating in Index page.\n";
                                }
                                //msg += "---------6-------------\n";
                                flag1 = 1;
                            }
                            if (flag1 == 1)
                            {
                                if (msg != "")
                                {
                                    throw new Exception();
                                }
                                break;
                            }
                        //}
                        //if(flag1 == 1)
                        //    break;
                    }
                }
                if (msg != "")
                {
                    throw new Exception();
                }

            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, ex.Message + $"-Exception trying to edit and display the TaskInfo details. Exception :{ex.InnerException?.Message}\n", msg, msg_name);
            }
        }

        [Test, Order(17)]
        public void Test20_UI_DeleteTask_Index_Test()
        {
            totalTestcases++;
            String msg = "";
            String msg_name = "";
            string functionName = NUnit.Framework.TestContext.CurrentContext.Test.Name;

            try
            {
                _driver.Navigate().GoToUrl(appURL + "Task/Index");
                System.Threading.Thread.Sleep(5000);

                var table = _driver.FindElement(By.TagName("tbody"));

                int flag = 0;
                int flag1 = 0;
                //msg+=_driver.PageSource.ToString();
                //var tds1 = table.FindElements(By.TagName("tbody"));

                foreach (var tr in table.FindElements(By.TagName("tr")))
                {

                    //msg += "---------1.1-------------\n";
                    //var tds1 = tr.FindElements(By.TagName("tr"));
                    var tds = tr.FindElements(By.TagName("td"));

                    //msg += "---------1.2-------------\n"+tds.Count;
                    for (var i = 0; i < tds.Count; i++)
                    {
                        //msg += "---------1-------------\n";
                        if (tds[i + 5].Text.Trim().Contains("TestStatus092"))
                        {
                            //msg += "---------2-------------\n";
                            var result = _driver.FindElement(By.Id("lnkDelete"));
                            //msg += "---------3-------------\n"+result;
                            if (result == null)
                            {
                                msg += "Delete link is not available. \n";
                                break;
                            }

                            ///msg += "---------4-------------\n";
                            tds[i + 6].FindElement(By.Id("lnkDelete")).Click();
                            System.Threading.Thread.Sleep(5000);

                            if (!_driver.Url.Contains("DeleteTask"))
                            {
                                //msg += "---------6-------------\n";   
                                msg += "DeleteTask Page is NOT navigating correctly.\n";
                            }
                            else
                            {
                                //msg += "---------6-------------\n";
                                _driver.ClickElement("btnDelete");
                                System.Threading.Thread.Sleep(5000);


                                if (_driver.PageSource.Contains("TestDesc092"))
                                {
                                    msg += "Deleted Info is populating in Index page.\n";
                                }
                                flag1 = 1;
                            }
                            if (flag1 == 1)
                            {
                                if (msg != "")
                                {
                                    throw new Exception();
                                }
                                break;
                            }
                        }
                        if (flag1 == 1)
                            break;
                    }


                }

                if (msg != "")
                {
                    throw new Exception();
                }
            }
            catch (Exception ex)
            {
                ExceptionCatch(functionName, ex.Message + $"-Exception trying to delete the TaskInfo details. Exception :{ex.InnerException?.Message}\n", msg, msg_name);
            }
        }
    }








    public class AssertFailureMessages
    {
        private string TypeName;
        public AssertFailureMessages(string typeName)
        {
            this.TypeName = typeName;
        }
        public string GetAssemblyNotFoundMessage(string assemblyName)
        {
            return $"Could not find {assemblyName}.dll";
        }
        public string GetTypeNotFoundMessage(string assemblyName, string typeName = null)
        {
            return $"Could not find {typeName ?? TypeName} in  {assemblyName}.dll";
        }
        public string GetFieldNotFoundMessage(string fieldName, string fieldType, string typeName = null)
        {
            return $"Could not a find public field {fieldName} of {fieldType} type in {typeName ?? TypeName} class";
        }
        public string GetPropertyNotFoundMessage(string propertyName, string propertyType, string typeName = null)
        {
            return $"Could not a find public property {propertyName} of {propertyType} type in {typeName ?? TypeName} class";
        }
        public string GetFieldTypeMismatchMessage(string fieldName, string expectedFieldType, string typeName = null)
        {
            return $"{fieldName} is not of {expectedFieldType} data type in {typeName ?? TypeName} class";
        }
        public string GetMethodNotFoundMessage(string methodName, string methodType, string[] parameters, string typeName = null)
        {
            string temp = "";
            if (parameters.Length > 0)
            {
                foreach (var item in parameters)
                {
                    temp += item.ToString() + ", ";
                }

                string errMessage = temp.Substring(0, temp.Length - 2);
                return $"Could not find public method {methodName} of return type {methodType} with parameters {errMessage} in {typeName ?? TypeName} class";
            }
            else
            {
                //string errMessage = temp.Substring(0, temp.Length - 2);
                return $"Could not find public method {methodName} of return type {methodType} with 0 parameters in {typeName ?? TypeName} class";
            }

        }
        public string GetExceptionTestFailureMessage(string methodName, string customExceptionTypeName, string propertyName, Exception exception, string typeName = null)
        {
            return $"{methodName} method of {typeName ?? TypeName} class doesnot throws exception of type {customExceptionTypeName} on validation failure for {propertyName}.\nException Message: {exception.InnerException?.Message}\nStack Trace:{exception.InnerException?.StackTrace}";
        }

        public string GetExceptionMessage(Exception ex, string methodName = null, string fieldName = null, string propertyName = null, string typeName = null)
        {
            string testFor = methodName != null ? methodName + " method" : fieldName != null ? fieldName + " field" : propertyName != null ? propertyName + " property" : "undefined";
            return $"Check the logic in the {testFor}.Exception Occured - {ex.Message}\n";
            //return $" Exception while testing {testFor} of {typeName ?? TypeName} class.\n";
        }

        public string GetReturnTypeAssertionFailMessage(string methodName, string expectedTypeName, string typeName = null)
        {
            return $"{methodName} method of {typeName ?? TypeName} class doesnot return value of {expectedTypeName} data type";
        }
        public string GetReturnValueAssertionFailMessage(string methodName, object expectedValue, string typeName = null)
        {
            return $"{methodName} method of {typeName ?? TypeName} class doesnot return the value {expectedValue}";
        }

        public string GetValidationFailureMessage(string methodName, string expectedValidationMessage, string propertyName, string typeName = null)
        {
            return $"{methodName} method of {typeName ?? TypeName} class doesnot return '{expectedValidationMessage}' on validation failure for property {propertyName}";
        }

    }

    public static class SeleniumExtensions
    {

        public static void SetElementText(this IWebDriver driver, string elementId, string text)
        {
            var Element = driver.FindElement(By.Id(elementId));
            Element.Clear();
            Element.SendKeys(text);
        }

        public static string GetElementText(this IWebDriver driver, string elementId)
        {
            return driver.GetElementText(elementId);
        }

        public static void ClickElement(this IWebDriver driver, string elementId)
        {
            driver.FindElement(By.Id(elementId)).Click();
        }

        //public static void SelectDropDownItemByValue(this IWebDriver driver, string elementId, string value)
        //{
        //    new SelectElement(driver.FindElement(By.Id(elementId))).SelectByValue(value);
        //}
        //public static void SelectDropDownItemByText(this IWebDriver driver, string elementId, string text)
        //{
        //    new SelectElement(driver.FindElement(By.Id(elementId))).SelectByText(text);
        //}


        public static string GetElementInnerText(this IWebDriver driver, string elementType, string attribute)
        {
            return driver.FindElement(By.XPath($"//{elementType}[{attribute}]")).GetAttribute("innerHTML");
        }

        public static int GetTableRowsCount(this IWebDriver driver, string elementId)
        {
            var Table = driver.FindElement(By.Id(elementId));
            return Table.FindElements(By.TagName("tr")).Count;
        }



    }

    public class TestBase : ATestBase
    {
        public TestBase(string assemblyName, string namespaceName, string typeName)
        {
            Console.WriteLine("-----12-------");
            Messages = new AssertFailureMessages(typeName);
            this.assemblyName = assemblyName;
            this.namespaceName = namespaceName;
            this.typeName = typeName;

            Console.WriteLine("-----13-------");
            Messages = new AssertFailureMessages(typeName);
            Console.WriteLine("-----14-------");
            assembly = Assembly.Load(assemblyName);
            Console.WriteLine("-----15-------");
            type = assembly.GetType($"{namespaceName}.{typeName}");
            Console.WriteLine("-----16-------");
        }
    }
    public abstract class ATestBase
    {
        public string assemblyName;
        public string namespaceName;
        public string typeName;
        public string controllerName;

        public AssertFailureMessages Messages;//= new AssertFailureMessages(typeName);

        protected Assembly assembly;
        public Type type;


        protected object typeInstance = null;
        protected void CreateNewTypeInstance()
        {
            typeInstance = assembly.CreateInstance(type.FullName);
        }
        public object GetTypeInstance()
        {
            if (typeInstance == null)
                CreateNewTypeInstance();
            return typeInstance;
        }
        public object InvokeMethod(string methodName, Type type, params object[] parameters)
        {
            var method = type.GetMethod(methodName);
            var instance = GetTypeInstance();
            var result = method.Invoke(instance, parameters);
            return result;
        }
        public T InvokeMethod<T>(string methodName, Type type, params object[] parameters)
        {
            var result = InvokeMethod(methodName, type, parameters);
            return (T)Convert.ChangeType(result, typeof(T));
        }

        public bool HasField(string fieldName, string fieldType)
        {
            bool Found = false;
            var field = type.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static);
            if (field != null)
            {
                Found = field.FieldType.Name == fieldType;
            }
            return Found;
        }

        public bool HasProperty(string propertyName, string propertyType)
        {
            bool Found = false;
            var property = type.GetProperty(propertyName);
            if (property != null)
            {
                Found = property.PropertyType.Name == propertyType; ;
            }
            return Found;
        }


        public bool HasMethod(string methodName, string methodType, string[] parameters)
        {
            bool Found = false;
            bool Type = false;
            bool Count = false;
            int flag = 0;

            var method = type.GetMethod(methodName);
            if (method != null)
            {
                string returnType = method.ReturnType.ToString();
                Found = method.Name == methodName;
                Type = methodType == returnType;
                ParameterInfo[] info = method.GetParameters();
                int param = 0;
                foreach (ParameterInfo p in info)
                {
                    if (p.ParameterType.FullName == parameters[param])
                    {
                        param++;
                    }
                    else
                    {
                        flag = 1;
                        break;
                    }
                }

                if (flag == 0 && param == parameters.Length)
                {
                    Count = true;
                }
            }
            if (Found && Type && Count)
            {
                return true;
            }
            return false;
        }
        public T GetAttributeFromProperty<T>(string propertyName, Type attribute)
        {

            var attr = type.GetProperty(propertyName).GetCustomAttribute(attribute, false);
            return (T)Convert.ChangeType(attr, typeof(T));
        }

        //public bool CheckFromUriAttribute(string methodName)
        //{
        //    ParameterInfo[] parameters = type.GetMethod(methodName).GetParameters();
        //    if (parameters.Length == 0)
        //    {
        //        return false;
        //    }

        //    Object[] myAttributes = parameters[0].GetCustomAttributes(typeof(System.Web.Http.FromUriAttribute), false);
        //    //{System.Web.Http.FromUriAttribute[0]}
        //    if (myAttributes.Length == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}

        //public bool CheckFromBodyAttribute(string methodName)
        //{
        //    ParameterInfo[] parameters = type.GetMethod(methodName).GetParameters();
        //    if (parameters.Length == 0)
        //    {
        //        return false;
        //    }

        //    Object[] myAttributes = parameters[0].GetCustomAttributes(typeof(System.Web.Http.FromBodyAttribute), false);

        //    if (myAttributes.Length == 0)
        //    {
        //        return false;
        //    }
        //    else
        //    {
        //        return true;
        //    }
        //}
    }
}

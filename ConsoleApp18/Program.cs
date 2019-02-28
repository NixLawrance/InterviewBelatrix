
using System;
using System.IO;
using System.Configuration;
using System.Data.SqlClient;
using System.Text;

namespace Belatrix.FileLogger
{
    /// <summary>

    /// MainApp startup class for Structural

    /// Strategy Design Pattern.

    /// </summary>

    class MainApp

    {
        /// <summary>

        /// Entry point into console application.

        /// </summary>

        static void Main()
        {
            Context context;

            // Three contexts following different strategies
            string error = "Unable to connect to the ERP";
            
            context = new Context(new ConcreteStrategyLogFile());
            context.JobLogger(error, MessageType.Error);
            context.JobLogger(error, MessageType.Warning);
            context.JobLogger(error, MessageType.Information);

            context = new Context(new ConcreteStrategyDatabase());
            context.JobLogger(error, MessageType.Error);
            context.JobLogger(error, MessageType.Warning);
            context.JobLogger(error, MessageType.Information);

            context = new Context(new ConcreteStrategyConsole());
            context.JobLogger(error, MessageType.Error);
            context.JobLogger(error, MessageType.Warning);
            context.JobLogger(error, MessageType.Information);


            // Wait for user

            Console.ReadKey();
        }
    }

    enum MessageType
    {
        Information = 1,
        Error,
        Warning
    }


    /// <summary>

    /// The 'Strategy' abstract class

    /// </summary>

    abstract class Strategy

    {
        public abstract void LogMessage(string message, MessageType messageType);
    }

    /// <summary>

    /// A 'ConcreteStrategy' class

    /// </summary>

    class ConcreteStrategyLogFile : Strategy

    {
        public override void LogMessage(string message, MessageType messageType)
        {
            try
            {
                string path = ConfigurationManager.AppSettings["FileLocation"] + string.Format("{0:yyyy-MM-dd}", DateTime.Now) + ".txt";
                using (StreamWriter sw = File.AppendText(path))
                {
                    StringBuilder pathLocationBuilder = new StringBuilder();
                    pathLocationBuilder.Append(messageType.ToString());
                    pathLocationBuilder.Append(" - ");
                    pathLocationBuilder.Append(DateTime.Now.ToShortDateString());
                    pathLocationBuilder.Append(" - ");
                    pathLocationBuilder.Append(message);
                    sw.WriteLine(pathLocationBuilder.ToString());
                }
            }
            catch (Exception ex) {
                throw ex;
            }
        }
    }

    /// <summary>

    /// A 'ConcreteStrategy' class

    /// </summary>

    class ConcreteStrategyDatabase : Strategy

    {
        public override void LogMessage(string message, MessageType messageType)
        {

            using (SqlConnection connection = new SqlConnection(ConfigurationManager.ConnectionStrings["ConnectionLogger"].ConnectionString))
            {
                try
                {
                    connection.Open();
                    StringBuilder commandBuilder = new StringBuilder();
                    commandBuilder.Append("Insert into LogMessage Values('");
                    commandBuilder.Append(message);
                    commandBuilder.Append("', ");
                    commandBuilder.Append((int)messageType);
                    commandBuilder.Append(")");

                    SqlCommand command = new SqlCommand(commandBuilder.ToString() , connection);
                    command.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    throw ex;
                } 
            } 
        }
    }

    /// <summary>

    /// A 'ConcreteStrategy' class

    /// </summary>

    class ConcreteStrategyConsole : Strategy

    {
        public override void LogMessage(string message, MessageType messageType)
        { 
            if (messageType == MessageType.Error)
            {
                Console.ForegroundColor = ConsoleColor.Red;
            }
            if (messageType ==  MessageType.Warning)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
            }
            if (messageType == MessageType.Information)
            {
                Console.ForegroundColor = ConsoleColor.White;
            }
            Console.WriteLine(DateTime.Now.ToShortDateString() + " " + message);

        }
    }

    /// <summary>

    /// The 'Context' class

    /// </summary>

    class Context

    {
        private Strategy _strategy;

        // Constructor

        public Context(Strategy strategy)
        {
            this._strategy = strategy;
        }

        public void JobLogger(string message, MessageType messageType)
        {
            _strategy.LogMessage(message, messageType);
        }
    }
}


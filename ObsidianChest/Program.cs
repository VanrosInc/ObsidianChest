using System;
using System.Net;
using System.IO;
using System.Threading;
using System.Text;
using System.Diagnostics;
using System.Collections;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using WMCommandFramework;
using PL = PluginLoader;
using PluginLoader.Configuration;
using ConfigFramework;

namespace ObsidianChest
{
    class Program
    {
        public static Program INSTANCE = null;

        static void Main(string[] args) => new Program().Run(args);

        private PL.PluginLoader loader = null;
        private CommandProcessor processor = null;
        private ConfigFile configFile = null;
        private Thread updateThread = null;

        public Program()
        {
            INSTANCE = this;
            //Initialize loader and processor.
            loader = new PL.PluginLoader();
            processor = new CommandProcessor();
            processor.ApplicationName = new AppName(
                "ObsidianChest-Server", new CommandVersion(0, 0, 1, "ALPHA")
                );
            processor.DefaultBackgroundColor = Console.BackgroundColor;
            processor.DefaultForegroundColor = Console.ForegroundColor;
            processor.Message = new InputMessage(
                new InputMessage.Message($"{processor.ApplicationName.GetName()}", ConsoleColor.DarkCyan),
                new InputMessage.Message(" "),
                new InputMessage.Message("[Type 'help' for help]", ConsoleColor.DarkGreen),
                InputMessage.Message.ResetColor
                );
            //Add Commands here.


            //Initialize ConfigFile.
            configFile = new ConfigFile();
        }

        private void Run(string[] args)
        {
            //Update Config.
            configFile.UpdateConfig();
            Debug = GetServerConfig().Debug;
            try
            {
                updateThread = new Thread(() => 
                {
                    Thread inputThread = new Thread(() => 
                    {
                        while(true)
                        {
                            if (processor.ExitProcessor) break;
                            if (!WaitForInput)
                            {
                                processor.Process(true);
                            }
                        }
                    });
                    inputThread.Start();
                    while (true)
                    {
                        if (processor.ExitProcessor) break;
                        foreach (Action action in UpdateMethods)
                            action.Invoke();
                    }
                });
                loader.LoadPlugins();
                updateThread.Start();
            }
            catch (PL.DependencyNotFoundException dex) { }
            catch (Exception ex) { }
        }

        public CommandProcessor GetCommandProcessor() => processor;
        public ConfigFile.ServerConfigFile GetServerConfig() => configFile.ReadConfigFile();
        public bool Debug
        {
            get => GetCommandProcessor().DebugMode;
            set
            {
                GetCommandProcessor().DebugMode = value;
            }
        }
        internal bool WaitForInput = true;
        internal List<Action> UpdateMethods = new List<Action>();
    }

    public sealed class ConfigFile
    {
        private FileInfo configFile = null;
        private ConfigMode mode = ConfigMode.JSON;

        public ConfigFile()
        {
            configFile = new FileInfo($"{Directory.GetCurrentDirectory()}\\server.config");
            if (!configFile.Exists)
            {
                DirectoryInfo di = configFile.Directory;
                if (!di.Exists)
                    di.Create();
                configFile.Create().Close();
                WriteConfig();
            }
        }

        private void WriteConfig(ServerConfigFile scf = null)
        {
            if (scf == null)
            {
                using (StreamWriter writer = new StreamWriter(configFile.FullName))
                {
                    if (mode == ConfigMode.JSON)
                    {
                        writer.WriteLine(ConfigUtils.JSON.WriteJSON(new ServerConfigFile()));
                        writer.Flush();
                        writer.Close();
                    }
                    else if (mode == ConfigMode.YAML)
                    {
                        writer.WriteLine(ConfigUtils.YAML.WriteYAML(new ServerConfigFile()));
                        writer.Flush();
                        writer.Close();
                    }
                    else
                    {
                        writer.WriteLine(ConfigUtils.XML.WriteXML(new ServerConfigFile()));
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
            else
            {
                using (StreamWriter writer = new StreamWriter(configFile.FullName))
                {
                    if (mode == ConfigMode.JSON)
                    {
                        writer.WriteLine(ConfigUtils.JSON.WriteJSON(scf));
                        writer.Flush();
                        writer.Close();
                    }
                    else if (mode == ConfigMode.YAML)
                    {
                        writer.WriteLine(ConfigUtils.YAML.WriteYAML(scf));
                        writer.Flush();
                        writer.Close();
                    }
                    else

                    {
                        writer.WriteLine(ConfigUtils.XML.WriteXML(scf));
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
        }

        public ServerConfigFile ReadConfigFile()
        {
            using (StreamReader reader = new StreamReader(configFile.FullName))
            {
                if (mode == ConfigMode.JSON)
                    return ConfigUtils.JSON.ReadJSON<ServerConfigFile>(reader.ReadToEnd());
                else if (mode == ConfigMode.YAML)
                    return ConfigUtils.YAML.ReadYAML<ServerConfigFile>(reader.ReadToEnd());
                else
                    return ConfigUtils.XML.ReadXML<ServerConfigFile>(reader.ReadToEnd());
            }
        }

        public void UpdateConfig()
        {
            var x = ReadConfigFile();
            if (!(x.ConfigMode == mode))
            {
                mode = x.ConfigMode;
                WriteConfig(x);
            }
        }

        public sealed class ServerConfigFile
        {
            public bool Debug = false;
            public ConfigMode ConfigMode = ConfigMode.JSON;
        }
    }
}

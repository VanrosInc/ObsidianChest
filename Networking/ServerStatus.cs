using System;
using Newtonsoft.Json;

namespace ObsidianChest.Networking
{
	public class ServerStatus
	{
		[JsonProperty("version")]
		public ServerVersion Version { get; set; }

		[JsonProperty("players")]
		public PlayerList Players { get; set; }

		[JsonProperty("description")]
		public string Description { get; set; }

		[JsonProperty("favicon")]
		public string Icon { get; set; }

		[JsonIgnore]
		public TimeSpan Latency { get; set; }

		public ServerStatus() { }

		public ServerStatus(ServerVersion version, PlayerList players, string description, string icon)
		{

			Version = version;

			Players = players;

			Description = description;

			Icon = icon;

		}

		public class ServerVersion
		{

			[JsonProperty("name")]
			public string Name { get; set; }

			[JsonProperty("protocol")]
			public long ProtocolVersion { get; set; }

			public ServerVersion() { }

			public ServerVersion(string name, long protocolVersion)
			{
				Name = name;
				ProtocolVersion = protocolVersion;
			}
		}

		public class PlayerList
		{
			[JsonProperty("maxplayers")]

			public int MaxPlayers { get; set; }

			[JsonProperty("onlineplayers")]

			public int OnlinePlayers { get; set; }

			[JsonProperty("sampleplayers")]

			public Player[] Players { get; set; }

			public class Player
			{
				[JsonProperty("name")]
				public string Name { get; set; }

				[JsonProperty("id")]
				public string ID { get; set; }

				public Player() { }

				public Player(string name, string id)
				{
					Name = name;
					ID = id;
				}
			}

			public PlayerList() { }

			public PlayerList(int maxPlayers, int onlinePlayers, Player[] players)
			{
				MaxPlayers = maxPlayers;
				OnlinePlayers = onlinePlayers;
				Players = players;
			}


		}

		public override string ToString()
		{
			return JsonConvert.SerializeObject(this, Formatting.Indented);
		}
	}
}


using Terraria;
using TerrariaApi.Server;
using Terraria.ID;
using System;
using TShockAPI;
using Microsoft.Xna.Framework;
using Newtonsoft.Json;
using System.IO;
using System.Collections.Generic;

[ApiVersion(2, 1)]
public class BossBuffPlugin : TerrariaPlugin
{
    public override string Name => "Boss Buff Plugin";
    public override Version Version => new Version(1, 1);
    public override string Author => "ViiDuc";
    public override string Description => "Buff specific bosses based on configuration in a JSON file. Includes /bossreload command.";

    private Dictionary<int, BossConfig> bossConfigs;

    private static readonly List<int> defaultBossIds = new List<int>
    {
        NPCID.KingSlime,
        NPCID.EyeofCthulhu,
        NPCID.EaterofWorldsHead,
        NPCID.BrainofCthulhu,
        NPCID.QueenBee,
        NPCID.SkeletronHead,
        NPCID.WallofFlesh,
        NPCID.Retinazer,
        NPCID.Spazmatism,
        NPCID.TheDestroyer,
        NPCID.SkeletronPrime,
        NPCID.Plantera,
        NPCID.Golem,
        NPCID.DukeFishron,
        NPCID.CultistBoss,
        NPCID.MoonLordCore,
        NPCID.QueenSlimeBoss,
        NPCID.HallowBoss,
        NPCID.Deerclops
    };

    public BossBuffPlugin(Main game) : base(game)
    {
    }

    public override void Initialize()
    {
        LoadBossConfig();

        ServerApi.Hooks.NpcSpawn.Register(this, OnNpcSpawn);

        Commands.ChatCommands.Add(new Command("bossbuff.reload", ReloadBossConfig, "bossreload"));
    }

    private void LoadBossConfig()
    {
        string configPath = Path.Combine(TShock.SavePath, "bossConfig.json");

        if (!File.Exists(configPath))
        {
            CreateDefaultConfig(configPath);
        }

        string json = File.ReadAllText(configPath);
        var configList = JsonConvert.DeserializeObject<List<BossConfig>>(json);

        bossConfigs = new Dictionary<int, BossConfig>();
        foreach (var config in configList)
        {
            bossConfigs[config.id] = config;
        }

        TShock.Log.Info("Boss configurations loaded.");
    }

    private void CreateDefaultConfig(string path)
    {
        var defaultConfig = new List<BossConfig>
        {
            new BossConfig { id = NPCID.KingSlime, name = "King Slime", hpMultiplier = 1.5f, strengthMultiplierOverride = 1.2f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.EyeofCthulhu, name = "Eye of Cthulhu", hpMultiplier = 2.0f, strengthMultiplierOverride = 1.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.EaterofWorldsHead, name = "Eater of Worlds", hpMultiplier = 1.8f, strengthMultiplierOverride = 1.3f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.BrainofCthulhu, name = "Brain of Cthulhu", hpMultiplier = 1.7f, strengthMultiplierOverride = 1.4f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.QueenBee, name = "Queen Bee", hpMultiplier = 2.0f, strengthMultiplierOverride = 1.6f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.SkeletronHead, name = "Skeletron", hpMultiplier = 2.5f, strengthMultiplierOverride = 2.0f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.WallofFlesh, name = "Wall of Flesh", hpMultiplier = 3.0f, strengthMultiplierOverride = 2.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.Retinazer, name = "The Twins (Retinazer)", hpMultiplier = 2.2f, strengthMultiplierOverride = 1.8f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.Spazmatism, name = "The Twins (Spazmatism)", hpMultiplier = 2.2f, strengthMultiplierOverride = 1.8f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.TheDestroyer, name = "The Destroyer", hpMultiplier = 3.0f, strengthMultiplierOverride = 2.0f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.SkeletronPrime, name = "Skeletron Prime", hpMultiplier = 3.5f, strengthMultiplierOverride = 2.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.Plantera, name = "Plantera", hpMultiplier = 3.5f, strengthMultiplierOverride = 2.7f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.Golem, name = "Golem", hpMultiplier = 3.0f, strengthMultiplierOverride = 2.2f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.DukeFishron, name = "Duke Fishron", hpMultiplier = 4.0f, strengthMultiplierOverride = 3.0f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.CultistBoss, name = "Lunatic Cultist", hpMultiplier = 4.5f, strengthMultiplierOverride = 3.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.MoonLordCore, name = "Moon Lord", hpMultiplier = 5.0f, strengthMultiplierOverride = 4.0f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.QueenSlimeBoss, name = "Queen Slime", hpMultiplier = 3.0f, strengthMultiplierOverride = 2.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.HallowBoss, name = "Empress of Light", hpMultiplier = 4.0f, strengthMultiplierOverride = 3.5f, sizeScaleOverride = 1.5f },
            new BossConfig { id = NPCID.Deerclops, name = "Deerclops", hpMultiplier = 2.5f, strengthMultiplierOverride = 2.0f, sizeScaleOverride = 1.5f }
        };

        string json = JsonConvert.SerializeObject(defaultConfig, Formatting.Indented);
        Directory.CreateDirectory(Path.GetDirectoryName(path));
        File.WriteAllText(path, json);
    }

    private void OnNpcSpawn(NpcSpawnEventArgs args)
    {
        NPC npc = Main.npc[args.NpcId];

        if (bossConfigs.ContainsKey(npc.netID))
        {
            BossConfig config = bossConfigs[npc.netID];

            if (config.hpMultiplier == 0 || config.damageMultiplier == 0)
            {
                TShock.Utils.Broadcast($"{config.name} has spawned with normal stats.", Color.Yellow);
                return;
            }
 
            NPCSpawnParams spawnParams = new NPCSpawnParams
            {
                playerCountForMultiplayerDifficultyOverride = 1
                strengthMultiplierOverride = config.damageMultiplier 100f,
                sizeScaleOverride = 1.5f
            };

            npc.lifeMax = (int)(npc.lifeMax * config.hpMultiplier);
            npc.life = npc.lifeMax;

            npc.damage = (int)(npc.damage * spawnParams.strengthMultiplierOverride);

            TShock.Utils.Broadcast($"{config.name} has spawned! HP: {npc.lifeMax}/{npc.lifeMax}, Damage: x{spawnParams.strengthMultiplierOverride}, Size: x{spawnParams.sizeScaleOverride}", Color.Red);
        }
    }

    private void ReloadBossConfig(CommandArgs args)
    {
        LoadBossConfig();
        args.Player.SendSuccessMessage("Boss configuration reloaded.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            ServerApi.Hooks.NpcSpawn.Deregister(this, OnNpcSpawn);
        }
        base.Dispose(disposing);
    }
}

public class BossConfig
{
    public int id { get; set; }
    public string name { get; set; }
    public float hpMultiplier { get; set; }
    public float damageMultiplier { get; set; }
}

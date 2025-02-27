﻿using HomeSpeaker.Shared;
using System.Text.Json;

namespace HomeSpeaker.Server
{
    public class LifecycleEvents : IHostedService
    {
        public LifecycleEvents(ILogger<LifecycleEvents> logger, IMusicPlayer player, IConfiguration config)
        {
            this.logger = logger;
            this.player = player;
            this.config = config;
        }

        //write to media folder because that exists outside of the container
        public string LastStatePath => Path.Combine(config[ConfigKeys.MediaFolder], "lastState.json");

        private readonly ILogger<LifecycleEvents> logger;
        private readonly IMusicPlayer player;
        private readonly IConfiguration config;

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Application started event raised!");
            if (File.Exists(LastStatePath))
            {
                logger.LogInformation("Found {LastStatePath} file, re-setting current song and queue", LastStatePath);

                var lastState = JsonSerializer.Deserialize<LastState>(await File.ReadAllTextAsync(LastStatePath));
                player.PlaySong(lastState.CurrentSong.Path);
                foreach (var s in lastState.Queue)
                {
                    player.EnqueueSong(s.Path);
                }

                logger.LogInformation("Restarted using {lastState}", lastState);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            logger.LogInformation("Application Stopping event raised!");
            if (player.Status.StillPlaying)
            {
                logger.LogInformation("Still playing music...saving current song and queue");
                var lastState = new LastState
                {
                    CurrentSong = player.Status.CurrentSong,
                    Queue = player.SongQueue
                };
                var json = JsonSerializer.Serialize(lastState);
                await File.WriteAllTextAsync(LastStatePath, json);
                logger.LogInformation("Saved {LastStatePath} with {LastState}", LastStatePath, lastState);
            }
            else //if we're not playing anything right now
            {
                logger.LogInformation("Not playing anything, no state to save.");
                if (File.Exists(LastStatePath)) //don't leave behind a file as if we were.
                    File.Delete(LastStatePath);
            }
        }

        public class LastState
        {
            public Song CurrentSong { get; set; }
            public IEnumerable<Song> Queue { get; set; }
        }
    }
}

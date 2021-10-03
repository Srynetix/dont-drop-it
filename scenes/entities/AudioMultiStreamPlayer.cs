using Godot;
using Godot.Collections;

public class AudioMultiStreamPlayer : Node
{
    [Export]
    public int MaxVoices = 4;

    private Array<AudioStreamPlayer> _Players = new Array<AudioStreamPlayer>();

    public AudioStreamPlayer GetVoice(int voice)
    {
        return _Players[voice];
    }

    public override void _Ready()
    {
        for (int i = 0; i < MaxVoices; ++i)
        {
            var player = new AudioStreamPlayer();
            AddChild(player);

            _Players.Add(player);
        }
    }

    public void Play(AudioStream stream, int voice = -1)
    {
        if (voice == -1)
        {
            var available = FindAvailablePlayer();
            if (available != null)
            {
                PlayUsingPlayer(stream, available);
            }
            else
            {
                PlayUsingPlayer(stream, FindOldestActivePlayer());
            }
        }
        else
        {
            PlayUsingPlayer(stream, _Players[voice]);
        }
    }

    public void PlayUsingPlayer(AudioStream stream, AudioStreamPlayer player)
    {
        player.Stop();

        player.Stream = stream;
        player.Play();
    }

    private AudioStreamPlayer FindAvailablePlayer()
    {
        foreach (var player in _Players)
        {
            if (!player.Playing)
            {
                return player;
            }
        }

        return null;
    }

    private AudioStreamPlayer FindOldestActivePlayer()
    {
        var oldestPlayer = _Players[0];
        var playbackPos = oldestPlayer.GetPlaybackPosition();

        for (int i = 1; i < MaxVoices; ++i)
        {
            var player = _Players[i];
            var pos = player.GetPlaybackPosition();
            if (pos > playbackPos)
            {
                playbackPos = pos;
                oldestPlayer = player;
            }
        }

        return oldestPlayer;
    }
}

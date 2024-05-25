﻿using Newtonsoft.Json;

namespace MorMor.Music.QQ;

public class MusicQQ
{
    private const string Uri = "https://api.lolimi.cn/API/yiny/";
    public static async Task<List<MusicInfo>> GetMusicList(string name)
    {
        var ret = new List<MusicInfo>();
        var param = new Dictionary<string, string>()
        {
            { "word", name },
        };
        var res = MomoAPI.Utils.Utils.HttpGet(Uri, param).Result;
        var data = JsonConvert.DeserializeObject<MusicList>(res);
        if (data != null && data.code == 200)
        {
            return data.data;
        }
        return ret;
    }

    public static async Task<MusicData?> GetMusic(string name, int id)
    {
        var param = new Dictionary<string, string>()
        {
            { "word", name },
            { "n", id.ToString()},
            { "q", "4"}
        };
        var res = await MomoAPI.Utils.Utils.HttpGet(Uri, param);
        var data = JsonConvert.DeserializeObject<Music>(res);
        if (data != null && data.code == 200)
        {
            return data.data;
        }
        return null;
    }
}

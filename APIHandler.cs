using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;
using Godot;
using HttpClient = System.Net.Http.HttpClient;

namespace CVSS_Overlay;

[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")] // no, it can't, this is made to be public across the app!
public partial class ApiHandler : Control {
    private Config _cfg;
    private HttpClient _scl;

    public ApiHandler(MainController main) {
        if (!File.Exists("api.config")) {
            GenerateConfig("api.config");
        }
        _cfg = ReadConfig("api.config");
        _scl = new()
        {
            BaseAddress = new Uri(_cfg.BaseUrl),
        };
    }

    private static Config ReadConfig(string f) { 
        using FileStream stream = File.OpenRead(f);
        return JsonSerializer.Deserialize<Config>(stream);
    }

    private static void GenerateConfig(string cfg) {
        using FileStream stream = File.Create(cfg);
        JsonSerializer.Serialize(stream, new Config {
            BaseUrl = "http://localhost:4444"
        });
    }

    public override void _Ready() {
        GD.Print(GetServerVersion());
    }

    public override void _EnterTree() {
        
    }

    public void Remove() {
        
    }
    
    // ------------------------------- HTTP ACCESS METHODS ---------------------------------------

    public string GetServerVersion() {
        return _scl.GetStringAsync("/").Result;
    }

    public string GetEventStreamAddress() {
        return "ws://" + new Uri(_cfg.BaseUrl).GetComponents(UriComponents.HostAndPort,UriFormat.UriEscaped) + "/stream/event";
    }
    
    public string GetTimeStreamAddress() {
        return "ws://" + new Uri(_cfg.BaseUrl).GetComponents(UriComponents.HostAndPort,UriFormat.UriEscaped) + "/stream/time";
    }
    
    // ------------------------------- /HTTP ACCESS METHODS --------------------------------------
    
    public class Config
    {
        public string BaseUrl { get; set; }
    }
}
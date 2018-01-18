using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using LitJson;
using UnityEngine;
using UnityEditor;

using Object = UnityEngine.Object;

//// zfc.exe -i D:\Project\Unity\OptimizeConfig\OptimizeConfig.csproj -o "ZeroFormatterGenerated.cs"
public class ColumDefine
{
    public string name;
    public Type type;
}

public class ConfigGeneratorWindow : EditorWindow
{
    [MenuItem("EditorTools/Generate Config")]
    private static void MenuItem_GenerateConfig()
    {
        ConfigGeneratorWindow.ShowWindow();
    }

    public static ConfigGeneratorWindow Instance;

    private static void ShowWindow()
    {
        Instance = EditorWindow.GetWindow<ConfigGeneratorWindow>();
        Instance.titleContent = new GUIContent("Generate Config");
    }

    private Dictionary<ZeroFormatterBuildStage.Stage, Action> _callbackDictionary;

    void Awake()
    {
        ClearStage();
    }

    void OnEnable()
    {
        InitCallbacks();
    }

    private void InitCallbacks()
    {
        _callbackDictionary = new Dictionary<ZeroFormatterBuildStage.Stage, Action>();
        _callbackDictionary.Add(ZeroFormatterBuildStage.Stage.TriggerBuild, BuildStageTriggerBuild);
        _callbackDictionary.Add(ZeroFormatterBuildStage.Stage.GenerateClassFiles, BuildStageGenerateClassFile);
        _callbackDictionary.Add(ZeroFormatterBuildStage.Stage.GenerateZeroFormatter, BuildStageGenerateZeroFormatter);
        _callbackDictionary.Add(ZeroFormatterBuildStage.Stage.GenerateBytes, BuildStageGenerateBytes);
        _callbackDictionary.Add(ZeroFormatterBuildStage.Stage.CreateAssetBundle, BuildStageCreateAssetBundle);
    }

    void Update()
    {
        if (IsBuildRunnging())
        {
            if (EditorApplication.isCompiling || EditorApplication.isUpdating)
                return;

            ZeroFormatterBuildStage.Stage stage = GetBuildStage();

            try
            {
                _callbackDictionary[stage]();
            }
            catch (Exception e)
            {
                string errorMessage = string.Format("!!! Build Error stage: {0}, error: {1}", stage, e);

                Debug.LogErrorFormat(errorMessage);

                ClearStage();
            }
        }
    }

    private void BuildStageTriggerBuild()
    {
        // Debug.Log("!!! BuildStageTriggerBuild");
        SetStage(ZeroFormatterBuildStage.Stage.GenerateClassFiles);
    }

    private void BuildStageGenerateClassFile()
    {
        // Debug.Log("!!! BuildStageGenerateClassFile");

        GenerateClassFile();

        AssetDatabase.Refresh();
        SetStage(ZeroFormatterBuildStage.Stage.GenerateZeroFormatter);
    }


    private void BuildStageGenerateZeroFormatter()
    {
        // Debug.Log("!!! BuildStageGenerateZeroFormatter");

        GenerateZeroFomatter();

        AssetDatabase.Refresh();
        SetStage(ZeroFormatterBuildStage.Stage.GenerateBytes);
    }

    private void BuildStageGenerateBytes()
    {
        // Debug.Log("!!! BuildStageGenerateBytes");

        GenerateBytes();
        
        AssetDatabase.Refresh();
        SetStage(ZeroFormatterBuildStage.Stage.CreateAssetBundle);
    }

    private void BuildStageCreateAssetBundle()
    {
        // Debug.Log("!!! BuildStageCreateAssetBundle");

        CreateAssetBundle();

        AssetDatabase.Refresh();
        ClearStage();
    }

    void OnGUI()
    {
        DrawBuildButton();
    }

    private void DrawBuildButton()
    {
        bool buildPressed = GUILayout.Button("Generate");
        if (buildPressed)
        {
            SetStage(ZeroFormatterBuildStage.Stage.TriggerBuild);
        }
    }

    private void ClearStage()
    {
        EditorPrefs.DeleteKey(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_RUNNING);
        EditorPrefs.DeleteKey(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_IDENTIFIER);
    }

    private bool IsBuildRunnging()
    {
        return EditorPrefs.GetBool(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_RUNNING, false);
    }

    private void SetBuildRunning(bool running)
    {
        EditorPrefs.SetBool(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_RUNNING, running);
    }

    private ZeroFormatterBuildStage.Stage GetBuildStage()
    {
        string stageStr = EditorPrefs.GetString(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_IDENTIFIER, "None");

        var stage = GetEnum<ZeroFormatterBuildStage.Stage>(stageStr);
        return stage;
    }

    private void SetStage(ZeroFormatterBuildStage.Stage stage)
    {
        SetBuildRunning(true);
        EditorPrefs.SetString(ZeroFormatterBuildStage.ZERO_FORMATTER_BUILD_STAGE_IDENTIFIER, stage.ToString());
    }

    public static T GetEnum<T>(string name)
    {
        T result = (T)System.Enum.Parse(typeof(T), name);

        return result;
    }

    private static float DoubleToFloat(double value)
    {
        return System.Convert.ToSingle(value);
    }

    private static string TypeToString(Type type)
    {
        if (type == typeof(bool))
        {
            return "bool";
        }
        else if (type == typeof(float))
        {
            return "float";
        }
        else if (type == typeof(int))
        {
            return "int";
        }
        else if (type == typeof(long))
        {
            return "long";
        }
        else if (type == typeof(string))
        {
            return "string";
        }

        return null;
    }

    private static Type GetJsonDataType(JsonData data)
    {
        if (data.IsBoolean)
        {
            return typeof(bool);
        }
        else if (data.IsDouble)
        {
            return typeof(float);
        }
        else if (data.IsInt)
        {
            return typeof(int);
        }
        else if (data.IsLong)
        {
            return typeof(long);
        }
        else if (data.IsString)
        {
            return typeof(string);
        }

        return null;
    }

    // =============================

    public static string ConfigDir = "Assets/JSONs";

    public static string ProtoClassDir = "Assets/ZeroFormatterGenerated/ConfigClass";
    public static string ProtoBytesDir = "Assets/ZeroFormatterGenerated/ConfigBytes";

    public static string CLASS_FORMAT_FILE = @"using ZeroFormatter;

namespace ClientConfig
{{
    [ZeroFormattable]
    public class {0} : ConfigGetItem<{0}, {1}>
    {{
        [Index(0)]
        public virtual ILazyDictionary<string, byte[]> _internalDictionary {{ get; set; }}

        protected override ILazyDictionary<string, byte[]> GetInternalDictionary()
        {{
            return _internalDictionary;
        }}
        
        protected override void SetInternalDictionary(ILazyDictionary<string, byte[]> dictionary)
        {{
            _internalDictionary = dictionary;
        }}
    }}
}}";

    public static string ITEM_FORMAT_FILE = @"using ZeroFormatter;

namespace ClientConfig
{{
    [ZeroFormattable]
    public class {0}
    {{
    {1}
    }}
}}
";

    public static string ITEM_FORMAT_ROW = @"
    [Index({0})]
    public virtual {1} {2} {{ get; set; }}
";

    private void GenerateClassFile()
    {
        if (Directory.Exists(ProtoClassDir))
        {
            Directory.Delete(ProtoClassDir, true);
        }
        Directory.CreateDirectory(ProtoClassDir);

        foreach (var fileInfo in new DirectoryInfo(ConfigDir).GetFiles("*.json", SearchOption.TopDirectoryOnly))
        {
            try
            {
                if(!fileInfo.Name.StartsWith("EditorOnly-"))
                    GenerateClassInternal(fileInfo);
            }
            catch (Exception e)
            {
                Debug.LogError("!!! error parse: " + fileInfo.Name);
                Debug.LogException(e);
            }
        }
    }

    private void GenerateClassInternal(FileInfo fileInfo)
    {
        /*
         * There are five basic value types supported by JSON Schema:
            string.
            number.
            integer.
            boolean.
            null.

            // 3 types: string int long float
            */
        string jsonStr = File.ReadAllText(fileInfo.FullName);

        JsonData jsons = JsonMapper.ToObject(jsonStr);

        int length = jsons.Count;
        if (length > 0)
        {
            List<ColumDefine> lists = GetJsonDefines(jsons);

            string className = Path.GetFileNameWithoutExtension(fileInfo.Name);
            className = UpperCaseHeader(className);

            string classItemName = className + "Item";
            // generate item
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < lists.Count; ++i)
            {
                string content = string.Format(ITEM_FORMAT_ROW, i, TypeToString(lists[i].type), lists[i].name);
                sb.Append(content);
            }

            string itemContent = string.Format(ITEM_FORMAT_FILE, classItemName, sb.ToString());
            string itemPath = Path.Combine(ProtoClassDir, classItemName + ".cs");
            File.WriteAllText(itemPath, itemContent);

            // generate class
            string classContent = string.Format(CLASS_FORMAT_FILE, className, classItemName);
            string classPath = Path.Combine(ProtoClassDir, className + ".cs");
            File.WriteAllText(classPath, classContent);
        }
    }

    private static string UpperCaseHeader(string className)
    {
        var sb = new StringBuilder();
        sb.Append(className[0].ToString().ToUpper());

        int length = className.Length;
        for (int i = 1; i < length; ++i)
        {
            sb.Append(className[i]);
        }

        return sb.ToString();
    }

    private List<ColumDefine> GetJsonDefines(JsonData jsons)
    {
        var lists = new List<ColumDefine>();

        JsonData json = jsons[0];
        foreach (var key in json.Keys)
        {
            JsonData value = json[key];

            // Debug.LogFormat("!!! {0} : {1} type: {2}", key, value, GetJsonDataType(value));

            ColumDefine define = new ColumDefine()
            {
                name = key,
                type = GetJsonDataType(value)
            };

            lists.Add(define);
        }

        return lists;
    }

    private void GenerateZeroFomatter()
    {
        string zfcPath = new FileInfo("Assets/3rd_party/ZeroFormatter/zfc.exe").FullName;
        string csprojPath = new FileInfo("Emily.csproj").FullName;
        string outputPath = new FileInfo("Assets/ZeroFormatterGenerated/ConfigZeroFormatter.cs").FullName;

        string command = string.Format("{0} -i {1} -o {2}", zfcPath, csprojPath, outputPath);
        Utils.ExecuteCommandSync(command);
    }

    private void CallStaticMethod(Type type, string methodName)
    {
        try
        {
            type.GetMethod(methodName, BindingFlags.Public | BindingFlags.Static).Invoke(null, null);
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    private void GenerateBytes()
    {
        if (Directory.Exists(ProtoBytesDir))
        {
            Directory.Delete(ProtoBytesDir, true);
        }
        Directory.CreateDirectory(ProtoBytesDir);

        Type type = Utils.NameToType("ZeroFormatter.ZeroFormatterInitializer");
        if (type != null)
        {
            CallStaticMethod(type, "Register");
        }
        else
        {
            Debug.LogError("can not find type ZeroFormatter.ZeroFormatterInitializer");
        }

        foreach (var fileInfo in new DirectoryInfo(ConfigDir).GetFiles("*.json", SearchOption.TopDirectoryOnly))
        {
            try
            {
                if (!fileInfo.Name.StartsWith("EditorOnly-"))
                    GenerateBytesInternal(fileInfo);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
        }
    }

    private static void GenerateBytesInternal(FileInfo fileInfo)
    {
        string jsonStr = File.ReadAllText(fileInfo.FullName);
        string outputPath = Path.Combine(ProtoBytesDir, Path.GetFileNameWithoutExtension(fileInfo.Name) + ".bytes");

        string className = Path.GetFileNameWithoutExtension(fileInfo.Name);
        className = UpperCaseHeader(className);

        Type classType = Utils.NameToType("ClientConfig." + className);

        object obj = Activator.CreateInstance(classType);
        var methodInfo = classType.GetMethod("SerializeObject");
        if(methodInfo != null)
            methodInfo.Invoke(obj, new object[] { jsonStr, outputPath });
    }

    private void CreateAssetBundle()
    {
        List<Object> lists = new List<Object>();

        foreach (var path in Directory.GetFiles(ProtoBytesDir, "*.bytes"))
        {
            var obj = AssetDatabase.LoadAssetAtPath<Object>(path);

            lists.Add(obj);
        }

        EditorUtils.CreateAssetBundle(lists.ToArray(), "config", "client_config.assetbundle");
    }
}

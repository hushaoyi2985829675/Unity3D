using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using UnityEditor;
using UnityEngine;
using OfficeOpenXml;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Math;
using Unity.VisualScripting;
using UnityEditor.Compilation;
using UnityEngine.Tilemaps;
using Object = System.Object;

public class LoadExcel
{
    private static string headType;
    private static ExcelWorksheet worksheet;
    private static string filePath = "Assets/Resources/Configs/Excel";
    private static readonly Queue<string> pendingExcelPaths = new Queue<string>();
    private static bool isListeningCompilation = false;
    [MenuItem("Tools/导表")]
    [MenuItem("Assets/导表", false, 100)]
    private static void importExcel()
    {
        if (Selection.objects.Length <= 0 || Selection.objects.Length > 1)
        {
            Debug.LogError("请选择一个文件");
            return;
        }
        
        var file = Selection.objects[0];
        string path = AssetDatabase.GetAssetPath(file);
        if (!string.Equals(Path.GetExtension(path), ".xlsx"))
        {
            Debug.LogError("请选择一个表");
            return;
        }

        ReadConfig(path);
    }
    private static void importExcel2()
    {
        if (Selection.objects.Length <= 0 || Selection.objects.Length > 1)
        {
            Debug.LogError("请选择一个文件");
            return;
        }
        
        var file = Selection.objects[0];
        string path = AssetDatabase.GetAssetPath(file);
        if (!string.Equals(Path.GetExtension(path), ".xlsx"))
        {
            Debug.LogError("请选择一个表");
            return;
        }

        ReadConfig(path);
    }
    [MenuItem("Tools/导全部表")]
    private static void importAllExcel()
    {
        string[] files = Directory.GetFiles(filePath, "*.xlsx", SearchOption.AllDirectories);
        foreach (string file in files)
        {
            ReadConfig(file);
        }
    }

    private static void ReadConfig(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            for (int i = 1; i <= excelPackage.Workbook.Worksheets.Count; i++)
            {
                worksheet = excelPackage.Workbook.Worksheets[i];
                StringBuilder scriptFIle = new StringBuilder();
                headType = "";   
                List<StringBuilder> classStrList = new List<StringBuilder>();
                //先把第一行循环一遍创建出数据结构
                for (int k = 1; k <= worksheet.Dimension.Columns; k++)
                {
                     StringBuilder sb = Create(worksheet,k);
                     classStrList.Add(sb);
                }
                scriptFIle.AppendLine("using System.Collections.Generic;\nusing UnityEngine;");
                scriptFIle.AppendLine();
                scriptFIle.AppendLine("namespace " + worksheet.Name + "Ns");
                scriptFIle.AppendLine("{");
                foreach (var classStr in classStrList )
                {
                    scriptFIle.Append(classStr.ToString());
                }
                scriptFIle.AppendLine();
                //创建ScriptTable
                scriptFIle.AppendLine("\tpublic class " + worksheet.Name + "Config: ScriptableObject");
                scriptFIle.AppendLine("\t{");
                string headName = char.ToLower(headType[0]) + headType.Substring(1) + "List";
                scriptFIle.AppendLine(string.Format("\t\tpublic List<{0}> {1} = new List<{2}>();",headType,headName,headType));
                scriptFIle.AppendLine("\t}");
                scriptFIle.AppendLine("}");
                string filePath = Application.dataPath + "/Resources/Configs/ExcelScript/" + worksheet.Name + "Config.cs";
                // 创建文件并写入内容，若文件已存在则会覆盖
                if (!File.Exists(filePath)) 
                {
                    FileStream fileStream = new FileStream(filePath, FileMode.OpenOrCreate);
                    fileStream.Close();
                }
                File.WriteAllText(filePath, scriptFIle.ToString());
            }

            // 刷新触发编译，后续的 ScriptableObject 创建和数据导入放到编译完成回调中
            AssetDatabase.Refresh();
            EnqueueExcelForPostCompile(path);
        }
    }

    private static void EnqueueExcelForPostCompile(string path)
    {
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        pendingExcelPaths.Enqueue(path);

        if (!isListeningCompilation)
        {
            CompilationPipeline.compilationFinished += OnCompilationFinished;
            isListeningCompilation = true;
        }
    }

    private static void OnCompilationFinished(object obj)
    {
        CompilationPipeline.compilationFinished -= OnCompilationFinished;
        isListeningCompilation = false;

        while (pendingExcelPaths.Count > 0)
        {
            string excelPath = pendingExcelPaths.Dequeue();
            try
            {
                ImportDataAfterCompile(excelPath);
            }
            catch (Exception e)
            {
                Debug.LogError(string.Format("导表数据时出错: {0}\n{1}", excelPath, e));
            }
        }
    }

    private static void ImportDataAfterCompile(string path)
    {
        FileInfo fileInfo = new FileInfo(path);
        if (!fileInfo.Exists)
        {
            Debug.LogError(string.Format("Excel 文件不存在: {0}", path));
            return;
        }

        using (ExcelPackage excelPackage = new ExcelPackage(fileInfo))
        {
            for (int i = 1; i <= excelPackage.Workbook.Worksheets.Count; i++)
            {
                worksheet = excelPackage.Workbook.Worksheets[i];

                string fullTypeName = string.Format("{0}.{1}, Assembly-CSharp", worksheet.Name + "Ns", worksheet.Name + "Config");
                Type t = Type.GetType(fullTypeName);
                if (t == null)
                {
                    Debug.LogError(string.Format("找不到类型: {0}，请确认脚本已成功编译。", fullTypeName));
                    continue;
                }

                string dataPath = "Assets/Resources/Configs/Data/" + worksheet.Name + "Config.asset";
                ScriptableObject script = ScriptableObject.CreateInstance(t);
                AssetDatabase.CreateAsset(script, dataPath);

                RefreshData(script);
                EditorUtility.SetDirty(script);
            }

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log(string.Format("导入成功: {0}", path));
        }
    }

    private static StringBuilder Create(ExcelWorksheet worksheet, int k)
    {
        string title = worksheet.Cells[2, k].Value.ToString();
        StringBuilder sb = new StringBuilder();
        if (title.StartsWith("#"))
        {
            title = title.Substring(1);
            if (string.IsNullOrEmpty(headType))
            {
                headType = title;
            }
            string classType = char.ToUpper(title[0]) + title.Substring(1);
            sb.AppendLine();
            sb.AppendLine("\t[System.Serializable]");
            sb.AppendLine($"\tpublic class {classType}");
            sb.AppendLine("\t{");
            CreateClass(worksheet,k + 1,sb);
            sb.AppendLine("\t}");
        }
        return sb;
    }

    private static void CreateClass(ExcelWorksheet worksheet,int k,StringBuilder sb)
    {
        if (k > worksheet.Dimension.Columns)
        {
            return;
        }
        string title = worksheet.Cells[2, k].Value.ToString();
        if (title.StartsWith("#"))
        {
            title = title.Substring(1);
            title = char.ToLower(title[0]) + title.Substring(1);
            string type = char.ToUpper(title[0]) + title.Substring(1);
            sb.AppendLine($"\t\tpublic List<{type}> {title}List;");
        }
        else
        {
            title = char.ToLower(title[0]) + title.Substring(1);
            string type = GetType(worksheet.Cells[3, k].Value.ToString());
            sb.AppendLine($"\t\tpublic {type} {title};");
            CreateClass(worksheet, k + 1,sb);
        }
    }

    static void RefreshData(ScriptableObject script)
    {
        for (int i = 4; i <= worksheet.Dimension.Rows; i++)
        {
            Assignment(i,1,script);
        }
    }

    static void Assignment(int i, int k,object obj)
    {
        if (k > worksheet.Dimension.Columns)
        {
            return;
        }
        string valueName = worksheet.Cells[2, k].Value.ToString();
       
        Type objType = obj.GetType();
        if (valueName.StartsWith("#"))
        {
            valueName = valueName.Substring(1);
            valueName = char.ToLower(valueName[0]) + valueName.Substring(1);
            FieldInfo listInfo = objType.GetField(valueName + "List");
            if (listInfo == null)
            {
                Debug.Log(string.Format("在类{0}没有{1}",objType.Name,valueName + "List"));
            }
            Type listType = listInfo.FieldType;
            Type elementType = listType.GetGenericArguments()[0];
            object listInstance = listInfo.GetValue(obj);
            if (listInstance == null)
            {
                // 创建 List 实例（如 new List<Item>()）
                listInstance = Activator.CreateInstance(listType);
                listInfo.SetValue(obj, listInstance);
            }
            object target = Activator.CreateInstance(elementType);
            MethodInfo addFunc = listType.GetMethod("Add");
            addFunc.Invoke(listInstance, new object[] {target});
            Assignment(i,k + 1,target);
        }
        else
        {
            //默认值
            string value = getDefaultValue(k);
            if (worksheet.Cells[i, k].Value != null)
            {
                value = worksheet.Cells[i, k].Value.ToString();
            }
            FieldInfo fieldInfo = objType.GetField(char.ToLower(valueName[0]) + valueName.Substring(1));
            object valueObj = GetValue(k,value);
            fieldInfo.SetValue(obj,valueObj);
            Assignment(i, k + 1, obj);
        }
    }

    static string GetType(string str)
    {
        switch (str)
        {
            case "int":
                return "int";
            case "float":
                return "float";
            case "bool":
                return "bool";
            case "Vector2":
                return "Vector2";
        }
        return "string"; 
    }

    static object GetValue(int k,string value)
    {
        string type = worksheet.Cells[3, k].Value.ToString();
        switch (type)
        {
            case "int":
                return int.Parse(value);
            case "float":
                return float.Parse(value);
            case "bool":
                if (value.Equals("false") || value.Equals("False"))
                {
                    return false;
                }
                return true;
            case "Vector2":
                string[] str = value.Split(",");
                return new Vector2(float.Parse(str[0]), float.Parse(str[1]));
        }
        return value; 
    }

    public static string getDefaultValue(int k)
    {
        string valueName = worksheet.Cells[3, k].Value.ToString();
        switch (valueName)
        {
            case "int":
            case "float":
            case "bool":
                return "0";
        }
        return ""; 
    }
}


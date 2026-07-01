# ISO 11820 建筑材料不燃性试验仿真系统 — 实施计划

> **For agentic workers:** REQUIRED SUB-SKILL: Use superpowers:subagent-driven-development (recommended) or superpowers:executing-plans to implement this plan task-by-task. Steps use checkbox (`- [ ]`) syntax for tracking.

**Goal:** 从零构建完整的 ISO 11820 建筑材料不燃性试验仿真 Windows 桌面应用。

**Architecture:** 分层自底向上 — 数据层 → 服务层 → 核心层 → 全局层 → UI 层。上层依赖下层，数据通过事件从下层传到上层，所有 UI 更新必须在 UI 线程执行。

**Tech Stack:** .NET 8, WinForms, SQLite (Microsoft.Data.Sqlite), OxyPlot.WindowsForms 2.x, EPPlus 7.x, PDFsharp-MigraDoc 6.x, Serilog 4.x, MathNet.Numerics 5.x, Microsoft.Extensions.Configuration 8.x

---

### Task 1: 创建项目骨架

**Files:**
- Create: `ISO_System/ISO11820.sln`
- Create: `ISO_System/ISO11820/ISO11820.csproj`
- Create: `ISO_System/ISO11820/Program.cs`
- Create: `ISO_System/ISO11820/appsettings.json`

- [ ] **Step 1: 创建解决方案文件**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System"
dotnet new sln -n ISO11820
```

Expected: `ISO11820.sln` created.

- [ ] **Step 2: 创建 WinForms 项目**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System"
dotnet new winforms -n ISO11820 -f net8.0
```

Expected: `ISO11820/` directory with `.csproj`, `Program.cs`, `Form1.cs` etc.

- [ ] **Step 3: 将项目添加到解决方案**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System"
dotnet sln add ISO11820/ISO11820.csproj
```

Expected: Project added to solution.

- [ ] **Step 4: 安装 NuGet 包**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet add package Microsoft.Data.Sqlite --version 8.0.*
dotnet add package Microsoft.Extensions.Configuration --version 8.0.*
dotnet add package Microsoft.Extensions.Configuration.Json --version 8.0.*
dotnet add package Serilog --version 4.0.*
dotnet add package Serilog.Sinks.File --version 6.0.*
dotnet add package OxyPlot.WindowsForms --version 2.1.*
dotnet add package EPPlus --version 7.1.*
dotnet add package PDFsharp-MigraDoc --version 6.1.*
dotnet add package MathNet.Numerics --version 5.0.*
```

Expected: All packages installed without errors.

- [ ] **Step 5: 创建目录结构**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
mkdir -p Models Data Services Core Global Forms
```

- [ ] **Step 6: 删除默认 Form1.cs**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
rm -f Form1.cs Form1.Designer.cs Form1.resx
```

- [ ] **Step 7: 编写 appsettings.json**

Write `ISO_System/ISO11820/appsettings.json`:
```json
{
  "Database": {
    "Provider": "Sqlite",
    "SqlitePath": "Data\\ISO11820.db"
  },
  "Hardware": {
    "ConstPower": 2048,
    "PidTemperature": 750,
    "SensorProtocol": "ModbusRtu"
  },
  "Simulation": {
    "EnableSimulation": true,
    "SimulateSensors": true,
    "SimulatePidController": true,
    "InitialFurnaceTemp": 720.0,
    "TargetFurnaceTemp": 750.0,
    "HeatingRatePerSecond": 40.0,
    "TempFluctuation": 0.5,
    "StableThreshold": 3.0,
    "SimulateFlame": false,
    "MaxTemperatureDriftPerTenMinutes": 2.0
  },
  "FileStorage": {
    "BaseDirectory": "D:\\ISO11820",
    "TestDataDirectory": "D:\\ISO11820\\TestData"
  },
  "Report": {
    "OutputDirectory": "D:\\ISO11820\\Reports",
    "EnablePdfExport": true
  }
}
```

- [ ] **Step 8: 确保 appsettings.json 复制到输出目录**

Read `ISO11820.csproj`, then add this ItemGroup if not present:
```xml
  <ItemGroup>
    <None Update="appsettings.json">
      <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
    </None>
  </ItemGroup>
```

- [ ] **Step 9: 编写 Program.cs 入口**

Write `ISO_System/ISO11820/Program.cs`:
```csharp
using ISO11820.Global;
using ISO11820.Forms;
using Microsoft.Extensions.Configuration;
using Serilog;

namespace ISO11820;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();

        // 初始化 Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.File("logs/iso11820-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        try
        {
            // 加载配置
            var config = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            AppContext.Instance.Configuration = config;
            AppContext.Instance.Initialize();

            Log.Information("系统启动完成");

            // 显示登录窗体
            Application.Run(new LoginForm());
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "系统启动失败");
            MessageBox.Show($"系统启动失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
```

- [ ] **Step 10: 创建 Models 目录占位**

所有模型类将在 Task 2 中创建。此步骤仅确保目录存在（已在 Step 5 完成）。

- [ ] **Step 11: 验证项目可编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds (with warnings about empty AppContext/LoginForm references if not yet created, but we'll fix in subsequent tasks). If it fails because AppContext and LoginForm don't exist yet, proceed — they'll be created in Tasks 2-5.

- [ ] **Step 12: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: create project skeleton with nuget packages and config"
```

---

### Task 2: 数据模型层

**Files:**
- Create: `ISO_System/ISO11820/Models/Operator.cs`
- Create: `ISO_System/ISO11820/Models/ProductMaster.cs`
- Create: `ISO_System/ISO11820/Models/Apparatus.cs`
- Create: `ISO_System/ISO11820/Models/Sensor.cs`
- Create: `ISO_System/ISO11820/Models/TestMaster.cs`
- Create: `ISO_System/ISO11820/Models/CalibrationRecord.cs`
- Create: `ISO_System/ISO11820/Models/TemperatureData.cs`
- Create: `ISO_System/ISO11820/Models/MasterMessage.cs`
- Create: `ISO_System/ISO11820/Models/DataBroadcastEventArgs.cs`

- [ ] **Step 1: Operator.cs**

Write `ISO_System/ISO11820/Models/Operator.cs`:
```csharp
namespace ISO11820.Models;

public class Operator
{
    public string UserId { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
}
```

- [ ] **Step 2: ProductMaster.cs**

Write `ISO_System/ISO11820/Models/ProductMaster.cs`:
```csharp
namespace ISO11820.Models;

public class ProductMaster
{
    public string ProductId { get; set; } = string.Empty;
    public string TestId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string Specification { get; set; } = string.Empty;
    public double Height { get; set; }
    public double Diameter { get; set; }
}
```

- [ ] **Step 3: Apparatus.cs**

Write `ISO_System/ISO11820/Models/Apparatus.cs`:
```csharp
namespace ISO11820.Models;

public class Apparatus
{
    public string ApparatusId { get; set; } = string.Empty;
    public string ApparatusName { get; set; } = string.Empty;
    public string CalibrationDate { get; set; } = string.Empty;
    public double ConstPower { get; set; }
    public string ComPort { get; set; } = string.Empty;
}
```

- [ ] **Step 4: Sensor.cs**

Write `ISO_System/ISO11820/Models/Sensor.cs`:
```csharp
namespace ISO11820.Models;

public class Sensor
{
    public string SensorId { get; set; } = string.Empty;
    public int ChannelId { get; set; }
    public string SensorName { get; set; } = string.Empty;
    public double RangeHigh { get; set; }
    public double RangeLow { get; set; }
}
```

- [ ] **Step 5: TestMaster.cs**

Write `ISO_System/ISO11820/Models/TestMaster.cs`:
```csharp
namespace ISO11820.Models;

public class TestMaster
{
    public string ProductId { get; set; } = string.Empty;
    public string TestId { get; set; } = string.Empty;
    public string TestDate { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double EnvTemp { get; set; }
    public double EnvHumidity { get; set; }
    public double PreWeight { get; set; }
    public double PostWeight { get; set; }
    public double LostWeight { get; set; }
    public double LostWeightPer { get; set; }
    public double DeltaTf { get; set; }
    public double DeltaTf1 { get; set; }
    public double DeltaTf2 { get; set; }
    public double DeltaTs { get; set; }
    public double DeltaTc { get; set; }
    public int TotalTestTime { get; set; }
    public int FlameDuration { get; set; }
    public int FlameStartTime { get; set; }
    public int HasFlame { get; set; }
    public string Remark { get; set; } = string.Empty;
    public string Flag { get; set; } = string.Empty;
    public string DurationMode { get; set; } = "Standard";
    public int TargetDuration { get; set; }
    public string ApparatusId { get; set; } = string.Empty;
    public string ApparatusName { get; set; } = string.Empty;
    public string ApparatusCalibrationDate { get; set; } = string.Empty;
    public double ConstPower { get; set; }
}
```

- [ ] **Step 6: CalibrationRecord.cs**

Write `ISO_System/ISO11820/Models/CalibrationRecord.cs`:
```csharp
namespace ISO11820.Models;

public class CalibrationRecord
{
    public int Id { get; set; }
    public string CalibrationDate { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;
    public double ReferenceTemp { get; set; }
    public double MeasuredTemp { get; set; }
    public double Deviation { get; set; }
}
```

- [ ] **Step 7: TemperatureData.cs**

Write `ISO_System/ISO11820/Models/TemperatureData.cs`:
```csharp
namespace ISO11820.Models;

public class TemperatureData
{
    public int Time { get; set; }
    public double Temp1 { get; set; }
    public double Temp2 { get; set; }
    public double TempSurface { get; set; }
    public double TempCenter { get; set; }
    public double TempCalibration { get; set; }
}
```

- [ ] **Step 8: MasterMessage.cs**

Write `ISO_System/ISO11820/Models/MasterMessage.cs`:
```csharp
namespace ISO11820.Models;

public class MasterMessage
{
    public string Time { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
}
```

- [ ] **Step 9: DataBroadcastEventArgs.cs**

Write `ISO_System/ISO11820/Models/DataBroadcastEventArgs.cs`:
```csharp
namespace ISO11820.Models;

public class DataBroadcastEventArgs : EventArgs
{
    public Dictionary<string, double> Temperatures { get; set; } = new();
    public string CurrentState { get; set; } = string.Empty;
    public int ElapsedSeconds { get; set; }
    public bool IsStable { get; set; }
    public double Drift { get; set; }
    public List<MasterMessage> Messages { get; set; } = new();
}
```

- [ ] **Step 10: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds (models have no dependencies).

- [ ] **Step 11: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add all data models"
```

---

### Task 3: 数据库层 — DbHelper + DbInitializer

**Files:**
- Create: `ISO_System/ISO11820/Data/DbHelper.cs`
- Create: `ISO_System/ISO11820/Data/DbInitializer.cs`
- Modify: `ISO_System/ISO11820/Global/AppContext.cs` (if exists, to wire up DbHelper)

- [ ] **Step 1: DbInitializer.cs — 建表和种子数据**

Write `ISO_System/ISO11820/Data/DbInitializer.cs`:
```csharp
using Microsoft.Data.Sqlite;

namespace ISO11820.Data;

public static class DbInitializer
{
    public static void Initialize(string connectionString)
    {
        using var conn = new SqliteConnection(connectionString);
        conn.Open();

        // 创建表
        var commands = new[]
        {
            @"CREATE TABLE IF NOT EXISTS operators (
                userid TEXT PRIMARY KEY,
                username TEXT NOT NULL UNIQUE,
                pwd TEXT NOT NULL,
                role TEXT NOT NULL
            )",

            @"CREATE TABLE IF NOT EXISTS productmaster (
                productid TEXT NOT NULL,
                testid TEXT NOT NULL,
                productname TEXT,
                specification TEXT,
                height REAL,
                diameter REAL,
                PRIMARY KEY (productid, testid)
            )",

            @"CREATE TABLE IF NOT EXISTS apparatus (
                apparatusid TEXT PRIMARY KEY,
                apparatusname TEXT,
                calibrationdate TEXT,
                constpower REAL,
                comport TEXT
            )",

            @"CREATE TABLE IF NOT EXISTS sensors (
                sensorid TEXT PRIMARY KEY,
                channelid INTEGER,
                sensorname TEXT,
                rangehigh REAL,
                rangelow REAL
            )",

            @"CREATE TABLE IF NOT EXISTS testmaster (
                productid TEXT NOT NULL,
                testid TEXT NOT NULL,
                testdate TEXT,
                operator TEXT,
                envtemp REAL,
                envhumidity REAL,
                preweight REAL,
                postweight REAL,
                lostweight REAL,
                lostweight_per REAL,
                deltatf REAL,
                deltatf1 REAL,
                deltatf2 REAL,
                deltats REAL,
                deltatc REAL,
                totaltesttime INTEGER,
                flameduration INTEGER,
                flamestarttime INTEGER,
                hasflame INTEGER DEFAULT 0,
                remark TEXT,
                flag TEXT,
                durationmode TEXT DEFAULT 'Standard',
                targetduration INTEGER DEFAULT 3600,
                apparatusid TEXT,
                apparatusname TEXT,
                apparatuscalibrationdate TEXT,
                constpower REAL,
                PRIMARY KEY (productid, testid)
            )",

            @"CREATE TABLE IF NOT EXISTS CalibrationRecords (
                id INTEGER PRIMARY KEY AUTOINCREMENT,
                calibrationdate TEXT,
                operator TEXT,
                referencetemp REAL,
                measuredtemp REAL,
                deviation REAL
            )"
        };

        foreach (var sql in commands)
        {
            using var cmd = new SqliteCommand(sql, conn);
            cmd.ExecuteNonQuery();
        }

        // 插入种子数据（仅当表为空时）
        SeedOperators(conn);
        SeedApparatus(conn);
        SeedSensors(conn);
    }

    private static void SeedOperators(SqliteConnection conn)
    {
        using var check = new SqliteCommand("SELECT COUNT(*) FROM operators", conn);
        if ((long)check.ExecuteScalar()! > 0) return;

        using var cmd = new SqliteCommand(
            @"INSERT INTO operators (userid, username, pwd, role) VALUES
              ('001', 'admin', '123456', 'admin'),
              ('002', 'experimenter', '123456', 'experimenter')", conn);
        cmd.ExecuteNonQuery();
    }

    private static void SeedApparatus(SqliteConnection conn)
    {
        using var check = new SqliteCommand("SELECT COUNT(*) FROM apparatus", conn);
        if ((long)check.ExecuteScalar()! > 0) return;

        using var cmd = new SqliteCommand(
            @"INSERT INTO apparatus (apparatusid, apparatusname, calibrationdate, constpower, comport)
              VALUES ('ISO11820-001', '不燃性试验炉', '2026-01-01', 2048, 'COM1')", conn);
        cmd.ExecuteNonQuery();
    }

    private static void SeedSensors(SqliteConnection conn)
    {
        using var check = new SqliteCommand("SELECT COUNT(*) FROM sensors", conn);
        if ((long)check.ExecuteScalar()! > 0) return;

        using var cmd = new SqliteCommand(
            @"INSERT INTO sensors (sensorid, channelid, sensorname, rangehigh, rangelow) VALUES
              ('TF1', 1, '炉温1', 1000, 0),
              ('TF2', 2, '炉温2', 1000, 0),
              ('TS',  3, '表面温', 1000, 0),
              ('TC',  4, '中心温', 1000, 0),
              ('TCal',5, '校准温', 1000, 0)", conn);
        cmd.ExecuteNonQuery();
    }
}
```

- [ ] **Step 2: DbHelper.cs — CRUD 封装**

Write `ISO_System/ISO11820/Data/DbHelper.cs`:
```csharp
using Microsoft.Data.Sqlite;
using ISO11820.Models;

namespace ISO11820.Data;

public class DbHelper
{
    private readonly string _connectionString;

    public DbHelper(string connectionString)
    {
        _connectionString = connectionString;
        DbInitializer.Initialize(connectionString);
    }

    private SqliteConnection CreateConnection()
    {
        var conn = new SqliteConnection(_connectionString);
        conn.Open();
        return conn;
    }

    // === 登录校验 ===
    public Operator? ValidateLogin(string username, string password)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            "SELECT userid, username, pwd, role FROM operators WHERE username = @u AND pwd = @p", conn);
        cmd.Parameters.AddWithValue("@u", username);
        cmd.Parameters.AddWithValue("@p", password);

        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Operator
            {
                UserId = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Role = reader.GetString(3)
            };
        }
        return null;
    }

    // === 设备信息 ===
    public Apparatus? GetApparatus(string apparatusId)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            "SELECT apparatusid, apparatusname, calibrationdate, constpower, comport FROM apparatus WHERE apparatusid = @id", conn);
        cmd.Parameters.AddWithValue("@id", apparatusId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new Apparatus
            {
                ApparatusId = reader.GetString(0),
                ApparatusName = reader.GetString(1),
                CalibrationDate = reader.GetString(2),
                ConstPower = reader.GetDouble(3),
                ComPort = reader.IsDBNull(4) ? "" : reader.GetString(4)
            };
        }
        return null;
    }

    // === 样品信息 ===
    public void InsertProduct(ProductMaster product)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            @"INSERT INTO productmaster (productid, testid, productname, specification, height, diameter)
              VALUES (@pid, @tid, @pn, @sp, @h, @d)", conn);
        cmd.Parameters.AddWithValue("@pid", product.ProductId);
        cmd.Parameters.AddWithValue("@tid", product.TestId);
        cmd.Parameters.AddWithValue("@pn", product.ProductName);
        cmd.Parameters.AddWithValue("@sp", product.Specification);
        cmd.Parameters.AddWithValue("@h", product.Height);
        cmd.Parameters.AddWithValue("@d", product.Diameter);
        cmd.ExecuteNonQuery();
    }

    public ProductMaster? GetProduct(string productId, string testId)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            "SELECT productid, testid, productname, specification, height, diameter FROM productmaster WHERE productid = @pid AND testid = @tid", conn);
        cmd.Parameters.AddWithValue("@pid", productId);
        cmd.Parameters.AddWithValue("@tid", testId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
        {
            return new ProductMaster
            {
                ProductId = reader.GetString(0),
                TestId = reader.GetString(1),
                ProductName = reader.IsDBNull(2) ? "" : reader.GetString(2),
                Specification = reader.IsDBNull(3) ? "" : reader.GetString(3),
                Height = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
                Diameter = reader.IsDBNull(5) ? 0 : reader.GetDouble(5)
            };
        }
        return null;
    }

    // === 试验主记录 ===
    public void InsertTestMaster(TestMaster tm)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            @"INSERT INTO testmaster (productid, testid, testdate, operator, envtemp, envhumidity,
              preweight, postweight, lostweight, lostweight_per, deltatf, deltatf1, deltatf2, deltats, deltatc,
              totaltesttime, flameduration, flamestarttime, hasflame, remark, flag,
              durationmode, targetduration, apparatusid, apparatusname, apparatuscalibrationdate, constpower)
              VALUES (@pid, @tid, @td, @op, @et, @eh,
              @pw, @pow, @lw, @lwp, @dtf, @dtf1, @dtf2, @dts, @dtc,
              @ttt, @fd, @fst, @hf, @rm, @fg,
              @dm, @tdur, @aid, @aname, @acal, @cp)", conn);
        cmd.Parameters.AddWithValue("@pid", tm.ProductId);
        cmd.Parameters.AddWithValue("@tid", tm.TestId);
        cmd.Parameters.AddWithValue("@td", tm.TestDate);
        cmd.Parameters.AddWithValue("@op", tm.Operator);
        cmd.Parameters.AddWithValue("@et", tm.EnvTemp);
        cmd.Parameters.AddWithValue("@eh", tm.EnvHumidity);
        cmd.Parameters.AddWithValue("@pw", tm.PreWeight);
        cmd.Parameters.AddWithValue("@pow", tm.PostWeight);
        cmd.Parameters.AddWithValue("@lw", tm.LostWeight);
        cmd.Parameters.AddWithValue("@lwp", tm.LostWeightPer);
        cmd.Parameters.AddWithValue("@dtf", tm.DeltaTf);
        cmd.Parameters.AddWithValue("@dtf1", tm.DeltaTf1);
        cmd.Parameters.AddWithValue("@dtf2", tm.DeltaTf2);
        cmd.Parameters.AddWithValue("@dts", tm.DeltaTs);
        cmd.Parameters.AddWithValue("@dtc", tm.DeltaTc);
        cmd.Parameters.AddWithValue("@ttt", tm.TotalTestTime);
        cmd.Parameters.AddWithValue("@fd", tm.FlameDuration);
        cmd.Parameters.AddWithValue("@fst", tm.FlameStartTime);
        cmd.Parameters.AddWithValue("@hf", tm.HasFlame);
        cmd.Parameters.AddWithValue("@rm", tm.Remark);
        cmd.Parameters.AddWithValue("@fg", tm.Flag);
        cmd.Parameters.AddWithValue("@dm", tm.DurationMode);
        cmd.Parameters.AddWithValue("@tdur", tm.TargetDuration);
        cmd.Parameters.AddWithValue("@aid", tm.ApparatusId);
        cmd.Parameters.AddWithValue("@aname", tm.ApparatusName);
        cmd.Parameters.AddWithValue("@acal", tm.ApparatusCalibrationDate);
        cmd.Parameters.AddWithValue("@cp", tm.ConstPower);
        cmd.ExecuteNonQuery();
    }

    public void UpdateTestMaster(TestMaster tm)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            @"UPDATE testmaster SET testdate=@td, operator=@op, envtemp=@et, envhumidity=@eh,
              preweight=@pw, postweight=@pow, lostweight=@lw, lostweight_per=@lwp,
              deltatf=@dtf, deltatf1=@dtf1, deltatf2=@dtf2, deltats=@dts, deltatc=@dtc,
              totaltesttime=@ttt, flameduration=@fd, flamestarttime=@fst, hasflame=@hf,
              remark=@rm, flag=@fg, durationmode=@dm, targetduration=@tdur,
              apparatusid=@aid, apparatusname=@aname, apparatuscalibrationdate=@acal, constpower=@cp
              WHERE productid=@pid AND testid=@tid", conn);
        cmd.Parameters.AddWithValue("@pid", tm.ProductId);
        cmd.Parameters.AddWithValue("@tid", tm.TestId);
        cmd.Parameters.AddWithValue("@td", tm.TestDate);
        cmd.Parameters.AddWithValue("@op", tm.Operator);
        cmd.Parameters.AddWithValue("@et", tm.EnvTemp);
        cmd.Parameters.AddWithValue("@eh", tm.EnvHumidity);
        cmd.Parameters.AddWithValue("@pw", tm.PreWeight);
        cmd.Parameters.AddWithValue("@pow", tm.PostWeight);
        cmd.Parameters.AddWithValue("@lw", tm.LostWeight);
        cmd.Parameters.AddWithValue("@lwp", tm.LostWeightPer);
        cmd.Parameters.AddWithValue("@dtf", tm.DeltaTf);
        cmd.Parameters.AddWithValue("@dtf1", tm.DeltaTf1);
        cmd.Parameters.AddWithValue("@dtf2", tm.DeltaTf2);
        cmd.Parameters.AddWithValue("@dts", tm.DeltaTs);
        cmd.Parameters.AddWithValue("@dtc", tm.DeltaTc);
        cmd.Parameters.AddWithValue("@ttt", tm.TotalTestTime);
        cmd.Parameters.AddWithValue("@fd", tm.FlameDuration);
        cmd.Parameters.AddWithValue("@fst", tm.FlameStartTime);
        cmd.Parameters.AddWithValue("@hf", tm.HasFlame);
        cmd.Parameters.AddWithValue("@rm", tm.Remark);
        cmd.Parameters.AddWithValue("@fg", tm.Flag);
        cmd.Parameters.AddWithValue("@dm", tm.DurationMode);
        cmd.Parameters.AddWithValue("@tdur", tm.TargetDuration);
        cmd.Parameters.AddWithValue("@aid", tm.ApparatusId);
        cmd.Parameters.AddWithValue("@aname", tm.ApparatusName);
        cmd.Parameters.AddWithValue("@acal", tm.ApparatusCalibrationDate);
        cmd.Parameters.AddWithValue("@cp", tm.ConstPower);
        cmd.ExecuteNonQuery();
    }

    public List<TestMaster> QueryTestMasters(string? productId = null, string? operatorName = null,
        string? startDate = null, string? endDate = null)
    {
        var results = new List<TestMaster>();
        using var conn = CreateConnection();
        var sql = "SELECT * FROM testmaster WHERE 1=1";
        if (!string.IsNullOrEmpty(productId))
            sql += " AND productid LIKE @pid";
        if (!string.IsNullOrEmpty(operatorName))
            sql += " AND operator = @op";
        if (!string.IsNullOrEmpty(startDate))
            sql += " AND testdate >= @sd";
        if (!string.IsNullOrEmpty(endDate))
            sql += " AND testdate <= @ed";
        sql += " ORDER BY testdate DESC";

        using var cmd = new SqliteCommand(sql, conn);
        if (!string.IsNullOrEmpty(productId))
            cmd.Parameters.AddWithValue("@pid", $"%{productId}%");
        if (!string.IsNullOrEmpty(operatorName))
            cmd.Parameters.AddWithValue("@op", operatorName);
        if (!string.IsNullOrEmpty(startDate))
            cmd.Parameters.AddWithValue("@sd", startDate);
        if (!string.IsNullOrEmpty(endDate))
            cmd.Parameters.AddWithValue("@ed", endDate);

        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(ReadTestMaster(reader));
        }
        return results;
    }

    public TestMaster? GetTestMaster(string productId, string testId)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            "SELECT * FROM testmaster WHERE productid = @pid AND testid = @tid", conn);
        cmd.Parameters.AddWithValue("@pid", productId);
        cmd.Parameters.AddWithValue("@tid", testId);
        using var reader = cmd.ExecuteReader();
        if (reader.Read())
            return ReadTestMaster(reader);
        return null;
    }

    private TestMaster ReadTestMaster(SqliteDataReader reader)
    {
        return new TestMaster
        {
            ProductId = reader.GetString(0),
            TestId = reader.GetString(1),
            TestDate = reader.IsDBNull(2) ? "" : reader.GetString(2),
            Operator = reader.IsDBNull(3) ? "" : reader.GetString(3),
            EnvTemp = reader.IsDBNull(4) ? 0 : reader.GetDouble(4),
            EnvHumidity = reader.IsDBNull(5) ? 0 : reader.GetDouble(5),
            PreWeight = reader.IsDBNull(6) ? 0 : reader.GetDouble(6),
            PostWeight = reader.IsDBNull(7) ? 0 : reader.GetDouble(7),
            LostWeight = reader.IsDBNull(8) ? 0 : reader.GetDouble(8),
            LostWeightPer = reader.IsDBNull(9) ? 0 : reader.GetDouble(9),
            DeltaTf = reader.IsDBNull(10) ? 0 : reader.GetDouble(10),
            DeltaTf1 = reader.IsDBNull(11) ? 0 : reader.GetDouble(11),
            DeltaTf2 = reader.IsDBNull(12) ? 0 : reader.GetDouble(12),
            DeltaTs = reader.IsDBNull(13) ? 0 : reader.GetDouble(13),
            DeltaTc = reader.IsDBNull(14) ? 0 : reader.GetDouble(14),
            TotalTestTime = reader.IsDBNull(15) ? 0 : reader.GetInt32(15),
            FlameDuration = reader.IsDBNull(16) ? 0 : reader.GetInt32(16),
            FlameStartTime = reader.IsDBNull(17) ? 0 : reader.GetInt32(17),
            HasFlame = reader.IsDBNull(18) ? 0 : reader.GetInt32(18),
            Remark = reader.IsDBNull(19) ? "" : reader.GetString(19),
            Flag = reader.IsDBNull(20) ? "" : reader.GetString(20),
            DurationMode = reader.IsDBNull(21) ? "Standard" : reader.GetString(21),
            TargetDuration = reader.IsDBNull(22) ? 3600 : reader.GetInt32(22),
            ApparatusId = reader.IsDBNull(23) ? "" : reader.GetString(23),
            ApparatusName = reader.IsDBNull(24) ? "" : reader.GetString(24),
            ApparatusCalibrationDate = reader.IsDBNull(25) ? "" : reader.GetString(25),
            ConstPower = reader.IsDBNull(26) ? 0 : reader.GetDouble(26)
        };
    }

    // === 校准记录 ===
    public void InsertCalibrationRecord(CalibrationRecord cr)
    {
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            @"INSERT INTO CalibrationRecords (calibrationdate, operator, referencetemp, measuredtemp, deviation)
              VALUES (@cd, @op, @rt, @mt, @dv)", conn);
        cmd.Parameters.AddWithValue("@cd", cr.CalibrationDate);
        cmd.Parameters.AddWithValue("@op", cr.Operator);
        cmd.Parameters.AddWithValue("@rt", cr.ReferenceTemp);
        cmd.Parameters.AddWithValue("@mt", cr.MeasuredTemp);
        cmd.Parameters.AddWithValue("@dv", cr.Deviation);
        cmd.ExecuteNonQuery();
    }

    public List<CalibrationRecord> GetCalibrationRecords()
    {
        var results = new List<CalibrationRecord>();
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand(
            "SELECT id, calibrationdate, operator, referencetemp, measuredtemp, deviation FROM CalibrationRecords ORDER BY id DESC", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new CalibrationRecord
            {
                Id = reader.GetInt32(0),
                CalibrationDate = reader.GetString(1),
                Operator = reader.GetString(2),
                ReferenceTemp = reader.GetDouble(3),
                MeasuredTemp = reader.GetDouble(4),
                Deviation = reader.GetDouble(5)
            });
        }
        return results;
    }

    // === 获取所有操作员（用于下拉）===
    public List<Operator> GetAllOperators()
    {
        var results = new List<Operator>();
        using var conn = CreateConnection();
        using var cmd = new SqliteCommand("SELECT userid, username, pwd, role FROM operators", conn);
        using var reader = cmd.ExecuteReader();
        while (reader.Read())
        {
            results.Add(new Operator
            {
                UserId = reader.GetString(0),
                Username = reader.GetString(1),
                Password = reader.GetString(2),
                Role = reader.GetString(3)
            });
        }
        return results;
    }
}
```

- [ ] **Step 3: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds. DbHelper and DbInitializer compile.

- [ ] **Step 4: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add database layer (DbHelper + DbInitializer)"
```

---

### Task 4: 仿真引擎 — SensorSimulator + DriftCalculator

**Files:**
- Create: `ISO_System/ISO11820/Services/SensorSimulator.cs`
- Create: `ISO_System/ISO11820/Services/DriftCalculator.cs`
- Create: `ISO_System/ISO11820/Models/SimulationConfig.cs`

- [ ] **Step 1: SimulationConfig.cs — 仿真配置类**

Write `ISO_System/ISO11820/Models/SimulationConfig.cs`:
```csharp
namespace ISO11820.Models;

public class SimulationConfig
{
    public bool EnableSimulation { get; set; } = true;
    public bool SimulateSensors { get; set; } = true;
    public bool SimulatePidController { get; set; } = true;
    public double InitialFurnaceTemp { get; set; } = 720.0;
    public double TargetFurnaceTemp { get; set; } = 750.0;
    public double HeatingRatePerSecond { get; set; } = 40.0;
    public double TempFluctuation { get; set; } = 0.5;
    public double StableThreshold { get; set; } = 3.0;
    public bool SimulateFlame { get; set; } = false;
    public double MaxTemperatureDriftPerTenMinutes { get; set; } = 2.0;
}
```

- [ ] **Step 2: SensorSimulator.cs — 5通道温度仿真算法**

Write `ISO_System/ISO11820/Services/SensorSimulator.cs`:
```csharp
using ISO11820.Models;

namespace ISO11820.Services;

public enum TestState
{
    Idle,
    Preparing,
    Ready,
    Recording,
    Complete
}

public class SensorSimulator
{
    private readonly SimulationConfig _config;
    private readonly Random _rng = new();

    public SensorSimulator(SimulationConfig config)
    {
        _config = config;
    }

    /// <summary>
    /// 每 800ms 调用一次，返回更新后的 5 通道温度字典。
    /// </summary>
    public Dictionary<string, double> Update(Dictionary<string, double> current, TestState state)
    {
        var result = new Dictionary<string, double>(current);
        double tf1 = current.GetValueOrDefault("TF1", _config.InitialFurnaceTemp);
        double tf2 = current.GetValueOrDefault("TF2", _config.InitialFurnaceTemp);
        double ts = current.GetValueOrDefault("TS", _config.InitialFurnaceTemp * 0.25);
        double tc = current.GetValueOrDefault("TC", _config.InitialFurnaceTemp * 0.2);
        double tcal = current.GetValueOrDefault("TCal", _config.InitialFurnaceTemp);

        double noise() => (_rng.NextDouble() * 2 - 1) * _config.TempFluctuation;

        // 停止状态（Idle）：降温
        if (state == TestState.Idle)
        {
            result["TF1"] = Math.Max(25, tf1 - 0.5 + noise() * 0.1);
            result["TF2"] = Math.Max(25, tf2 - 0.5 + noise() * 0.1);
            result["TS"] = Math.Max(25, tf1 * 0.3 + noise());
            result["TC"] = Math.Max(25, tf1 * 0.25 + noise());
            result["TCal"] = Math.Max(25, tf1 + noise() * 2);
            return result;
        }

        // Preparing / Ready / Recording / Complete
        bool isRecording = (state == TestState.Recording || state == TestState.Complete);

        // 炉温处理
        double target = _config.TargetFurnaceTemp;
        double stableThreshold = _config.StableThreshold;

        if (tf1 < target - stableThreshold)
        {
            // 升温阶段
            result["TF1"] = tf1 + _config.HeatingRatePerSecond * 0.8 + noise();
            result["TF2"] = tf2 + _config.HeatingRatePerSecond * 0.8 + noise();
        }
        else
        {
            // 稳定阶段：钳位到 750°C + 噪声
            result["TF1"] = target + noise();
            result["TF2"] = target + noise();
        }

        // 表面温和中心温
        if (isRecording)
        {
            double surfaceTarget = Math.Min(result["TF1"] * 0.95, 800);
            result["TS"] = ts + (surfaceTarget - ts) * 0.02 + noise();

            double centerTarget = Math.Min(result["TF1"] * 0.85, 750);
            result["TC"] = tc + (centerTarget - tc) * 0.01 + noise();
        }
        else
        {
            result["TS"] = result["TF1"] * 0.3 + noise();
            result["TC"] = result["TF1"] * 0.25 + noise();
        }

        // 校准温
        result["TCal"] = result["TF1"] + noise() * 2;

        return result;
    }

    /// <summary>获取初始温度字典</summary>
    public Dictionary<string, double> GetInitialTemperatures()
    {
        double t = _config.InitialFurnaceTemp;
        var rng = new Random();
        double noise() => (rng.NextDouble() * 2 - 1) * _config.TempFluctuation;
        return new Dictionary<string, double>
        {
            ["TF1"] = t + noise(),
            ["TF2"] = t + noise(),
            ["TS"] = t * 0.3 + noise(),
            ["TC"] = t * 0.25 + noise(),
            ["TCal"] = t + noise() * 2
        };
    }
}
```

- [ ] **Step 3: DriftCalculator.cs — 温漂计算**

Write `ISO_System/ISO11820/Services/DriftCalculator.cs`:
```csharp
using MathNet.Numerics;

namespace ISO11820.Services;

public static class DriftCalculator
{
    /// <summary>
    /// 对最近 10 分钟的温度数据做线性回归，返回斜率（°C/10min）。
    /// 至少需要 10 个数据点。
    /// </summary>
    public static double CalculateDrift(List<double> temperatures)
    {
        if (temperatures.Count < 10)
            return double.NaN;

        // 使用 MathNet.Numerics 进行线性回归
        double[] x = Enumerable.Range(0, temperatures.Count).Select(i => (double)i).ToArray();
        double[] y = temperatures.ToArray();

        var (slope, _) = SimpleRegression.Fit(x, y);

        // 斜率为每次采样的变化率，转换为 °C/10min
        // 假设每秒 1 次采样，600 个点 = 10 分钟
        double samplesPer10Min = 600.0;
        return slope * samplesPer10Min;
    }
}
```

- [ ] **Step 4: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds.

- [ ] **Step 5: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add simulation engine (SensorSimulator + DriftCalculator)"
```

---

### Task 5: 数据采集服务 — DaqWorker

**Files:**
- Create: `ISO_System/ISO11820/Services/DaqWorker.cs`

- [ ] **Step 1: DaqWorker.cs**

Write `ISO_System/ISO11820/Services/DaqWorker.cs`:
```csharp
using ISO11820.Models;

namespace ISO11820.Services;

public class DaqWorker : IDisposable
{
    private readonly System.Timers.Timer _timer;
    private readonly SensorSimulator _simulator;
    private readonly SimulationConfig _config;
    private Dictionary<string, double> _temperatures;
    private readonly List<double> _furnaceTempHistory = new(); // 最近 600 秒的炉温，用于温漂

    public event EventHandler<DataBroadcastEventArgs>? DataBroadcast;

    public Dictionary<string, double> Temperatures => _temperatures;
    public TestState CurrentState { get; set; } = TestState.Idle;
    public int ElapsedSeconds { get; set; }

    public DaqWorker(SensorSimulator simulator, SimulationConfig config)
    {
        _simulator = simulator;
        _config = config;
        _temperatures = simulator.GetInitialTemperatures();
        _timer = new System.Timers.Timer(800);
        _timer.Elapsed += OnTick;
        _timer.AutoReset = true;
    }

    public void Start() => _timer.Start();
    public void Stop() => _timer.Stop();

    public void ResetElapsed() => ElapsedSeconds = 0;

    private double _accumulatedMs;
    private readonly List<MasterMessage> _pendingMessages = new();

    private void OnTick(object? sender, System.Timers.ElapsedEventArgs e)
    {
        // 仿真模式：更新温度
        if (_config.EnableSimulation)
        {
            _temperatures = _simulator.Update(_temperatures, CurrentState);
        }

        // 记录炉温历史（用于温漂计算）
        _furnaceTempHistory.Add(_temperatures["TF1"]);
        if (_furnaceTempHistory.Count > 600)
            _furnaceTempHistory.RemoveAt(0);

        // 计算温漂
        double drift = DriftCalculator.CalculateDrift(_furnaceTempHistory);

        // 累计时间（每 800ms 一次，凑够 1 秒触发秒逻辑）
        _accumulatedMs += 800;
        bool secondElapsed = false;
        if (_accumulatedMs >= 1000)
        {
            _accumulatedMs -= 1000;
            secondElapsed = true;
        }

        // 构建广播数据
        var args = new DataBroadcastEventArgs
        {
            Temperatures = new Dictionary<string, double>(_temperatures),
            CurrentState = CurrentState.ToString(),
            ElapsedSeconds = ElapsedSeconds,
            IsStable = false,
            Drift = drift,
            Messages = new List<MasterMessage>(_pendingMessages)
        };

        // 触发事件
        DataBroadcast?.Invoke(this, args);

        // 清空已发送的消息
        _pendingMessages.Clear();
    }

    public void AddMessage(string message)
    {
        _pendingMessages.Add(new MasterMessage
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
            Message = message
        });
    }

    /// <summary>获取炉温历史列表（用于稳定判定）</summary>
    public List<double> GetFurnaceTempHistory() => new(_furnaceTempHistory);

    /// <summary>获取最近 10 分钟的炉温温漂</summary>
    public double GetCurrentDrift()
    {
        return DriftCalculator.CalculateDrift(_furnaceTempHistory);
    }

    public void Dispose()
    {
        _timer.Dispose();
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add data acquisition worker (DaqWorker)"
```

---

### Task 6: 试验控制器 — TestController (状态机)

**Files:**
- Create: `ISO_System/ISO11820/Core/TestController.cs`

- [ ] **Step 1: TestController.cs**

Write `ISO_System/ISO11820/Core/TestController.cs`:
```csharp
using ISO11820.Data;
using ISO11820.Models;
using ISO11820.Services;

namespace ISO11820.Core;

public class TestController
{
    private readonly DbHelper _db;
    private readonly DaqWorker _daqWorker;
    private readonly SimulationConfig _simConfig;

    public TestState State { get; private set; } = TestState.Idle;
    public TestMaster? CurrentTest { get; private set; }
    public int ElapsedSeconds { get; private set; }

    private int _stableTickCount;
    private readonly List<double> _pidOutputQueue = new(); // 恒功率值队列
    public List<TemperatureData> TemperatureHistory { get; } = new(); // 本次试验温度数据

    public event EventHandler<string>? StateChanged;
    public event EventHandler<MasterMessage>? MessageGenerated;

    public TestController(DbHelper db, DaqWorker daqWorker, SimulationConfig simConfig)
    {
        _db = db;
        _daqWorker = daqWorker;
        _simConfig = simConfig;
    }

    /// <summary>DaqWorker 每 800ms 调用此方法推进状态机。每秒子逻辑由 DaqWorker 通过 ElapsedSeconds 属性同步。</summary>
    public void DoWork()
    {
        if (State == TestState.Idle || CurrentTest == null) return;

        double tf1 = _daqWorker.Temperatures["TF1"];
        double tf2 = _daqWorker.Temperatures["TF2"];
        double drift = _daqWorker.GetCurrentDrift();
        bool isDriftValid = !double.IsNaN(drift);
        double maxDrift = _simConfig.MaxTemperatureDriftPerTenMinutes;

        // 稳定判定（Preparing / Ready 状态）
        if (State == TestState.Preparing)
        {
            bool inRange = tf1 >= (_simConfig.TargetFurnaceTemp - _simConfig.StableThreshold)
                        && tf1 <= (_simConfig.TargetFurnaceTemp + _simConfig.StableThreshold);

            if (inRange)
            {
                _stableTickCount++;
                if (_stableTickCount > 3) // 约 3.2 秒
                {
                    // 检查温漂是否满足
                    if (isDriftValid && Math.Abs(drift) <= maxDrift)
                    {
                        TransitionTo(TestState.Ready);
                        EmitMessage("温度已稳定，可以开始记录");
                    }
                }
            }
            else
            {
                _stableTickCount = 0;
            }

            // 记录 PID 输出值（仿真模式用炉温近似）
            _pidOutputQueue.Add(tf1);
            if (_pidOutputQueue.Count > 600) _pidOutputQueue.RemoveAt(0);
        }

        if (State == TestState.Ready)
        {
            // 稳定后继续记录 PID 值
            _pidOutputQueue.Add(tf1);
            if (_pidOutputQueue.Count > 600) _pidOutputQueue.RemoveAt(0);

            // 检查是否跌出稳定范围
            bool inRange = tf1 >= (_simConfig.TargetFurnaceTemp - _simConfig.StableThreshold)
                        && tf1 <= (_simConfig.TargetFurnaceTemp + _simConfig.StableThreshold);
            if (!inRange)
            {
                _stableTickCount = 0;
                TransitionTo(TestState.Preparing);
            }
        }

        if (State == TestState.Recording)
        {
            // 检查终止条件
            CheckTerminationCondition();
        }
    }

    /// <summary>当 DaqWorker 的秒计数器增加时调用</summary>
    public void OnSecondElapsed()
    {
        if (State != TestState.Recording || CurrentTest == null) return;

        ElapsedSeconds++;

        // 记录温度数据
        var temps = _daqWorker.Temperatures;
        TemperatureHistory.Add(new TemperatureData
        {
            Time = ElapsedSeconds,
            Temp1 = temps["TF1"],
            Temp2 = temps["TF2"],
            TempSurface = temps["TS"],
            TempCenter = temps["TC"],
            TempCalibration = temps["TCal"]
        });
    }

    private void CheckTerminationCondition()
    {
        if (CurrentTest == null) return;

        int targetDuration = CurrentTest.DurationMode == "Standard" ? 3600 : CurrentTest.TargetDuration;

        // 到达目标时长：无条件终止
        if (ElapsedSeconds >= targetDuration)
        {
            EmitMessage($"记录时间到达 {targetDuration} 秒，试验自动结束");
            TransitionTo(TestState.Complete);
            return;
        }

        // 标准模式：每 5 分钟检查提前终止（30、35、40、45、50、55 分钟）
        if (CurrentTest.DurationMode == "Standard" && ElapsedSeconds >= 1800 && ElapsedSeconds % 300 == 0)
        {
            double drift = _daqWorker.GetCurrentDrift();
            double maxDrift = _simConfig.MaxTemperatureDriftPerTenMinutes;

            if (!double.IsNaN(drift) && Math.Abs(drift) <= maxDrift)
            {
                EmitMessage("满足终止条件，试验结束");
                TransitionTo(TestState.Complete);
            }
        }
    }

    // === 按钮操作 ===

    public bool StartHeating()
    {
        if (State != TestState.Idle) return false;
        TransitionTo(TestState.Preparing);
        EmitMessage("开始升温，系统升温中");
        return true;
    }

    public bool StopHeating()
    {
        if (State != TestState.Preparing && State != TestState.Ready && State != TestState.Complete) return false;
        TransitionTo(TestState.Idle);
        EmitMessage("停止加热，系统冷却中");
        return true;
    }

    public bool StartRecording()
    {
        if (State != TestState.Ready) return false;

        // 计算恒功率值
        if (_pidOutputQueue.Count > 0)
        {
            double avgPower = _pidOutputQueue.Average();
            if (CurrentTest != null)
                CurrentTest.ConstPower = avgPower;
        }

        ElapsedSeconds = 0;
        TemperatureHistory.Clear();
        TransitionTo(TestState.Recording);
        EmitMessage("开始记录，计时开始");
        return true;
    }

    public bool StopRecording()
    {
        if (State != TestState.Recording) return false;

        if (ElapsedSeconds > 0)
        {
            EmitMessage("用户手动停止记录");
            TransitionTo(TestState.Complete);
        }
        else
        {
            TransitionTo(TestState.Preparing);
        }
        return true;
    }

    /// <summary>创建新试验并进入 Preparing 状态（前提是炉子已在 Preparing 状态）</summary>
    public void CreateTest(TestMaster test, ProductMaster product)
    {
        _db.InsertProduct(product);
        CurrentTest = test;
        ElapsedSeconds = 0;
        TemperatureHistory.Clear();
        _stableTickCount = 0;
        _pidOutputQueue.Clear();

        if (State == TestState.Idle)
        {
            TransitionTo(TestState.Preparing);
            EmitMessage("开始升温，系统升温中");
        }
        // 如果炉子已经在 Preparing 状态（上次试验保存后），不需要重新升温
    }

    /// <summary>保存试验记录（现象 + 质量）并标记完成</summary>
    public void SaveTestRecord(double postWeight, int hasFlame, int flameStartTime,
        int flameDuration, string remark)
    {
        if (CurrentTest == null) return;

        var temps = _daqWorker.Temperatures;
        double envTemp = CurrentTest.EnvTemp;

        CurrentTest.PostWeight = postWeight;
        CurrentTest.LostWeight = CurrentTest.PreWeight - postWeight;
        CurrentTest.LostWeightPer = CurrentTest.PreWeight > 0
            ? CurrentTest.LostWeight / CurrentTest.PreWeight * 100 : 0;
        CurrentTest.HasFlame = hasFlame;
        CurrentTest.FlameStartTime = flameStartTime;
        CurrentTest.FlameDuration = flameDuration;
        CurrentTest.Remark = remark;
        CurrentTest.TotalTestTime = ElapsedSeconds;
        CurrentTest.Flag = "10000000";

        // 温升计算
        CurrentTest.DeltaTf1 = temps["TF1"] - envTemp;
        CurrentTest.DeltaTf2 = temps["TF2"] - envTemp;
        CurrentTest.DeltaTs = temps["TS"] - envTemp;
        CurrentTest.DeltaTc = temps["TC"] - envTemp;
        CurrentTest.DeltaTf = CurrentTest.DeltaTs; // 综合温升取表面温升

        // 写入数据库
        _db.InsertTestMaster(CurrentTest);
    }

    /// <summary>清空当前试验缓存（保存后调用）</summary>
    public void ClearCurrentTest()
    {
        CurrentTest = null;
        ElapsedSeconds = 0;
        TemperatureHistory.Clear();
        _stableTickCount = 0;
        _pidOutputQueue.Clear();

        // 保持 Preparing 状态（炉子仍是热的）
        if (State == TestState.Complete)
            TransitionTo(TestState.Preparing);
    }

    /// <summary>是否有未保存的已完成试验</summary>
    public bool HasUnSavedCompleteTest()
    {
        return CurrentTest != null
            && CurrentTest.TotalTestTime > 0
            && CurrentTest.Flag != "10000000";
    }

    // === 内部方法 ===

    private void TransitionTo(TestState newState)
    {
        var oldState = State;
        State = newState;
        _daqWorker.CurrentState = newState;
        StateChanged?.Invoke(this, newState.ToString());
    }

    private void EmitMessage(string message)
    {
        var msg = new MasterMessage
        {
            Time = DateTime.Now.ToString("HH:mm:ss"),
            Message = message
        };
        _daqWorker.AddMessage(message);
        MessageGenerated?.Invoke(this, msg);
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add test controller with state machine"
```

---

### Task 7: 导出服务 — ExportService

**Files:**
- Create: `ISO_System/ISO11820/Services/ExportService.cs`

- [ ] **Step 1: ExportService.cs**

Write `ISO_System/ISO11820/Services/ExportService.cs`:
```csharp
using System.Text;
using ISO11820.Models;
using Microsoft.Extensions.Configuration;
using OfficeOpenXml;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

namespace ISO11820.Services;

public class ExportService
{
    private readonly IConfiguration _config;

    public ExportService(IConfiguration config)
    {
        _config = config;
        ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
    }

    private string BaseDir => _config["FileStorage:BaseDirectory"] ?? "D:\\ISO11820";
    private string TestDataDir => _config["FileStorage:TestDataDirectory"] ?? "D:\\ISO11820\\TestData";
    private string ReportDir => _config["Report:OutputDirectory"] ?? "D:\\ISO11820\\Reports";

    /// <summary>导出 CSV</summary>
    public string ExportCsv(TestMaster tm, List<TemperatureData> tempData)
    {
        string dir = Path.Combine(TestDataDir, tm.ProductId, tm.TestId);
        Directory.CreateDirectory(dir);

        string filePath = Path.Combine(dir, "sensor_data.csv");
        var sb = new StringBuilder();
        sb.AppendLine("Time,Temp1,Temp2,TempSurface,TempCenter,TempCalibration");
        foreach (var td in tempData)
        {
            sb.AppendLine($"{td.Time},{td.Temp1:F1},{td.Temp2:F1},{td.TempSurface:F1},{td.TempCenter:F1},{td.TempCalibration:F1}");
        }
        File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
        return filePath;
    }

    /// <summary>导出 Excel</summary>
    public string ExportExcel(TestMaster tm, List<TemperatureData> tempData)
    {
        Directory.CreateDirectory(ReportDir);
        string filePath = Path.Combine(ReportDir, $"{tm.TestId}_报告.xlsx");

        using var package = new ExcelPackage();
        // Sheet1: 试验信息
        var sheet1 = package.Workbook.Worksheets.Add("试验信息");
        sheet1.Cells["A1"].Value = "ISO 11820 不燃性试验报告";
        sheet1.Cells["A1"].Style.Font.Size = 16;
        sheet1.Cells["A1"].Style.Font.Bold = true;

        int row = 3;
        WriteInfoRow(sheet1, ref row, "样品编号", tm.ProductId);
        WriteInfoRow(sheet1, ref row, "试验标识", tm.TestId);
        WriteInfoRow(sheet1, ref row, "试验日期", tm.TestDate);
        WriteInfoRow(sheet1, ref row, "操作员", tm.Operator);
        WriteInfoRow(sheet1, ref row, "环境温度 (°C)", $"{tm.EnvTemp:F1}");
        WriteInfoRow(sheet1, ref row, "环境湿度 (%)", $"{tm.EnvHumidity:F1}");
        WriteInfoRow(sheet1, ref row, "试验前质量 (g)", $"{tm.PreWeight:F2}");
        WriteInfoRow(sheet1, ref row, "试验后质量 (g)", $"{tm.PostWeight:F2}");
        WriteInfoRow(sheet1, ref row, "失重率 (%)", $"{tm.LostWeightPer:F2}");
        WriteInfoRow(sheet1, ref row, "炉温1温升 (°C)", $"{tm.DeltaTf1:F1}");
        WriteInfoRow(sheet1, ref row, "炉温2温升 (°C)", $"{tm.DeltaTf2:F1}");
        WriteInfoRow(sheet1, ref row, "表面温升 (°C)", $"{tm.DeltaTs:F1}");
        WriteInfoRow(sheet1, ref row, "中心温升 (°C)", $"{tm.DeltaTc:F1}");
        WriteInfoRow(sheet1, ref row, "综合温升 (°C)", $"{tm.DeltaTf:F1}");
        WriteInfoRow(sheet1, ref row, "火焰持续时间 (s)", $"{tm.FlameDuration}");
        WriteInfoRow(sheet1, ref row, "总试验时长 (s)", $"{tm.TotalTestTime}");

        // 判定结论
        row++;
        string verdict = (tm.DeltaTf <= 50 && tm.LostWeightPer <= 50 && tm.FlameDuration < 5)
            ? "通过 — 材料判定为不燃" : "不通过";
        sheet1.Cells[$"A{row}"].Value = $"判定结论: {verdict}";
        sheet1.Cells[$"A{row}"].Style.Font.Bold = true;

        sheet1.Column(1).Width = 25;
        sheet1.Column(2).Width = 25;

        // Sheet2: 温度数据
        var sheet2 = package.Workbook.Worksheets.Add("温度数据");
        sheet2.Cells["A1"].Value = "Time (s)";
        sheet2.Cells["B1"].Value = "炉温1 (°C)";
        sheet2.Cells["C1"].Value = "炉温2 (°C)";
        sheet2.Cells["D1"].Value = "表面温 (°C)";
        sheet2.Cells["E1"].Value = "中心温 (°C)";
        sheet2.Cells["F1"].Value = "校准温 (°C)";
        for (int i = 0; i < tempData.Count; i++)
        {
            var td = tempData[i];
            int r = i + 2;
            sheet2.Cells[$"A{r}"].Value = td.Time;
            sheet2.Cells[$"B{r}"].Value = td.Temp1;
            sheet2.Cells[$"C{r}"].Value = td.Temp2;
            sheet2.Cells[$"D{r}"].Value = td.TempSurface;
            sheet2.Cells[$"E{r}"].Value = td.TempCenter;
            sheet2.Cells[$"F{r}"].Value = td.TempCalibration;
        }

        // Sheet3: 温度曲线图
        var sheet3 = package.Workbook.Worksheets.Add("温度曲线");
        var chart = sheet3.Drawings.AddChart("TemperatureChart", OfficeOpenXml.Drawing.Chart.eChartType.XYScatterLines);
        chart.Title.Text = "温度曲线";
        chart.SetPosition(0, 0, 0, 0);
        chart.SetSize(800, 500);

        // 为每条线添加系列
        AddChartSeries(chart, sheet2, tempData, 1, "炉温1");
        AddChartSeries(chart, sheet2, tempData, 2, "炉温2");
        AddChartSeries(chart, sheet2, tempData, 3, "表面温");
        AddChartSeries(chart, sheet2, tempData, 4, "中心温");

        package.SaveAs(new FileInfo(filePath));
        return filePath;
    }

    private void AddChartSeries(OfficeOpenXml.Drawing.Chart.ExcelChart chart, OfficeOpenXml.Worksheet sheet,
        List<TemperatureData> data, int colOffset, string name)
    {
        var series = chart.Series.Add(
            sheet.Cells[2, colOffset + 1, data.Count + 1, colOffset + 1],
            sheet.Cells[2, 1, data.Count + 1, 1]);
        series.Header = name;
    }

    private void WriteInfoRow(OfficeOpenXml.Worksheet sheet, ref int row, string label, string value)
    {
        sheet.Cells[$"A{row}"].Value = label;
        sheet.Cells[$"B{row}"].Value = value;
        row++;
    }

    /// <summary>导出 PDF</summary>
    public string ExportPdf(TestMaster tm, List<TemperatureData> tempData)
    {
        Directory.CreateDirectory(ReportDir);
        string filePath = Path.Combine(ReportDir, $"{tm.TestId}_报告.pdf");

        using var document = new PdfDocument();
        document.Info.Title = $"ISO 11820 试验报告 - {tm.TestId}";
        var page = document.AddPage();
        using var gfx = XGraphics.FromPdfPage(page);
        var fontTitle = new XFont("Microsoft YaHei", 16, XFontStyleEx.Bold);
        var fontNormal = new XFont("Microsoft YaHei", 11, XFontStyleEx.Regular);
        var fontBold = new XFont("Microsoft YaHei", 11, XFontStyleEx.Bold);

        double y = 40;
        gfx.DrawString("ISO 11820 不燃性试验报告", fontTitle, XBrushes.Black,
            new XRect(0, y, page.Width, 30), XStringFormats.TopCenter);
        y += 40;

        DrawPdfLine(gfx, ref y, "样品编号", tm.ProductId, fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "试验标识", tm.TestId, fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "试验日期", tm.TestDate, fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "操作员", tm.Operator, fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "失重率", $"{tm.LostWeightPer:F2}%", fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "综合温升", $"{tm.DeltaTf:F1} °C", fontNormal, page.Width);
        DrawPdfLine(gfx, ref y, "试验时长", $"{tm.TotalTestTime} 秒", fontNormal, page.Width);

        y += 20;
        string verdict = (tm.DeltaTf <= 50 && tm.LostWeightPer <= 50 && tm.FlameDuration < 5)
            ? "判定结论: 通过 — 材料判定为不燃" : "判定结论: 不通过";
        gfx.DrawString(verdict, fontBold, XBrushes.Black,
            new XRect(50, y, page.Width - 100, 25), XStringFormats.TopLeft);

        document.Save(filePath);
        return filePath;
    }

    private void DrawPdfLine(XGraphics gfx, ref double y, string label, string value, XFont font, double pageWidth)
    {
        gfx.DrawString($"{label}: {value}", font, XBrushes.Black,
            new XRect(50, y, pageWidth - 100, 22), XStringFormats.TopLeft);
        y += 22;
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add export service (CSV/Excel/PDF)"
```

---

### Task 8: 全局上下文 — AppContext

**Files:**
- Create: `ISO_System/ISO11820/Global/AppContext.cs`

- [ ] **Step 1: AppContext.cs**

Write `ISO_System/ISO11820/Global/AppContext.cs`:
```csharp
using ISO11820.Core;
using ISO11820.Data;
using ISO11820.Models;
using ISO11820.Services;
using Microsoft.Extensions.Configuration;

namespace ISO11820.Global;

public sealed class AppContext
{
    public static AppContext Instance { get; } = new();
    private AppContext() { }

    public IConfiguration Configuration { get; set; } = null!;
    public DbHelper Db { get; private set; } = null!;
    public SimulationConfig SimulationConfig { get; private set; } = null!;
    public SensorSimulator Simulator { get; private set; } = null!;
    public DaqWorker DaqWorker { get; private set; } = null!;
    public TestController TestController { get; private set; } = null!;
    public ExportService ExportService { get; private set; } = null!;

    public string CurrentOperator { get; set; } = string.Empty;
    public string CurrentRole { get; set; } = string.Empty;

    public void Initialize()
    {
        // 读取仿真配置
        SimulationConfig = new SimulationConfig();
        Configuration.GetSection("Simulation").Bind(SimulationConfig);

        // 初始化数据库
        string dbPath = Configuration["Database:SqlitePath"] ?? "Data\\ISO11820.db";
        string fullPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, dbPath);
        string? dbDir = Path.GetDirectoryName(fullPath);
        if (!string.IsNullOrEmpty(dbDir))
            Directory.CreateDirectory(dbDir);

        string connStr = $"Data Source={fullPath}";
        Db = new DbHelper(connStr);

        // 初始化服务
        Simulator = new SensorSimulator(SimulationConfig);
        DaqWorker = new DaqWorker(Simulator, SimulationConfig);
        TestController = new TestController(Db, DaqWorker, SimulationConfig);
        ExportService = new ExportService(Configuration);

        // 创建文件存储目录
        string baseDir = Configuration["FileStorage:BaseDirectory"] ?? "D:\\ISO11820";
        string testDataDir = Configuration["FileStorage:TestDataDirectory"] ?? "D:\\ISO11820\\TestData";
        string reportDir = Configuration["Report:OutputDirectory"] ?? "D:\\ISO11820\\Reports";
        Directory.CreateDirectory(baseDir);
        Directory.CreateDirectory(testDataDir);
        Directory.CreateDirectory(reportDir);
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds. All layers now compile.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add global application context singleton"
```

---

### Task 9: 登录窗体 — LoginForm

**Files:**
- Create: `ISO_System/ISO11820/Forms/LoginForm.cs`
- Create: `ISO_System/ISO11820/Forms/LoginForm.Designer.cs`

- [ ] **Step 1: LoginForm.cs**

Write `ISO_System/ISO11820/Forms/LoginForm.cs`:
```csharp
using ISO11820.Global;

namespace ISO11820.Forms;

public partial class LoginForm : Form
{
    private RadioButton rbAdmin = null!;
    private RadioButton rbExperimenter = null!;
    private TextBox txtPassword = null!;
    private Button btnLogin = null!;
    private Button btnCancel = null!;
    private Label lblTitle = null!;
    private Label lblPassword = null!;
    private GroupBox gbRole = null!;

    public LoginForm()
    {
        InitializeComponent();
    }

    private void InitializeComponent()
    {
        this.Text = "ISO 11820 不燃性试验系统 — 登录";
        this.Size = new Size(420, 320);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(240, 240, 240);

        lblTitle = new Label
        {
            Text = "ISO 11820 建筑材料不燃性试验系统",
            Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
            Size = new Size(380, 35),
            Location = new Point(20, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };

        gbRole = new GroupBox
        {
            Text = "选择角色",
            Font = new Font("Microsoft YaHei", 10),
            Size = new Size(360, 65),
            Location = new Point(20, 65)
        };

        rbAdmin = new RadioButton
        {
            Text = "管理员",
            Location = new Point(30, 28),
            Size = new Size(80, 22),
            Checked = true,
            Font = new Font("Microsoft YaHei", 10)
        };

        rbExperimenter = new RadioButton
        {
            Text = "试验员",
            Location = new Point(130, 28),
            Size = new Size(80, 22),
            Font = new Font("Microsoft YaHei", 10)
        };

        gbRole.Controls.Add(rbAdmin);
        gbRole.Controls.Add(rbExperimenter);

        lblPassword = new Label
        {
            Text = "输入密码:",
            Font = new Font("Microsoft YaHei", 10),
            Location = new Point(20, 145),
            Size = new Size(80, 25)
        };

        txtPassword = new TextBox
        {
            Location = new Point(110, 143),
            Size = new Size(200, 28),
            PasswordChar = '*',
            Font = new Font("Microsoft YaHei", 10)
        };

        btnLogin = new Button
        {
            Text = "登 录",
            Location = new Point(80, 200),
            Size = new Size(100, 38),
            Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
            BackColor = Color.FromArgb(0, 122, 204),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnLogin.FlatAppearance.BorderSize = 0;
        btnLogin.Click += BtnLogin_Click;

        btnCancel = new Button
        {
            Text = "退 出",
            Location = new Point(220, 200),
            Size = new Size(100, 38),
            Font = new Font("Microsoft YaHei", 10),
            FlatStyle = FlatStyle.Flat
        };
        btnCancel.Click += (s, e) => Application.Exit();

        this.Controls.Add(lblTitle);
        this.Controls.Add(gbRole);
        this.Controls.Add(lblPassword);
        this.Controls.Add(txtPassword);
        this.Controls.Add(btnLogin);
        this.Controls.Add(btnCancel);

        this.AcceptButton = btnLogin;
        txtPassword.KeyDown += (s, e) => { if (e.KeyCode == Keys.Enter) BtnLogin_Click(s, e); };
    }

    private void BtnLogin_Click(object? sender, EventArgs e)
    {
        string username = rbAdmin.Checked ? "admin" : "experimenter";
        string password = txtPassword.Text;

        var op = AppContext.Instance.Db.ValidateLogin(username, password);
        if (op != null)
        {
            AppContext.Instance.CurrentOperator = op.Username;
            AppContext.Instance.CurrentRole = op.Role;

            this.Hide();
            var mainForm = new MainForm();
            mainForm.FormClosed += (s, args) => this.Close();
            mainForm.Show();
        }
        else
        {
            MessageBox.Show("密码错误，请重新输入", "登录失败",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            txtPassword.SelectAll();
            txtPassword.Focus();
        }
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds (MainForm reference ok — will be created next).

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add login form"
```

---

### Task 10: 主窗体 — MainForm（温度显示 + 曲线图 + 消息日志 + 按钮控制 + Tab布局）

**Files:**
- Create: `ISO_System/ISO11820/Forms/MainForm.cs`

- [ ] **Step 1: MainForm.cs — 完整主界面**

This is the largest file. Write `ISO_System/ISO11820/Forms/MainForm.cs`:

```csharp
using ISO11820.Core;
using ISO11820.Global;
using ISO11820.Models;
using ISO11820.Services;
using OxyPlot;
using OxyPlot.Series;
using OxyPlot.WindowsForms;
using OxyPlot.Axes;

namespace ISO11820.Forms;

public partial class MainForm : Form
{
    private readonly AppContext _ctx = AppContext.Instance;
    private readonly TestController _tc;

    // 温度显示 Label
    private Label lblTF1 = null!, lblTF2 = null!, lblTS = null!, lblTC = null!, lblTCal = null!;
    private Label lblTF1Val = null!, lblTF2Val = null!, lblTSVal = null!, lblTCVal = null!, lblTCalVal = null!;
    private Label lblStatus = null!, lblTimer = null!, lblDrift = null!, lblSample = null!;

    // 按钮
    private Button btnNewTest = null!, btnStartHeat = null!, btnStopHeat = null!;
    private Button btnStartRecord = null!, btnStopRecord = null!, btnTestRecord = null!;
    private Button btnSettings = null!;

    // 曲线图
    private PlotView plotView = null!;
    private PlotModel plotModel = null!;
    private LineSeries seriesTF1 = null!, seriesTF2 = null!, seriesTS = null!, seriesTC = null!;
    private readonly List<double> _timePoints = new();
    private int _timeCounter;

    // 消息日志
    private RichTextBox rtbLog = null!;

    // Tab控件
    private TabControl tabControl = null!;
    private TabPage tabMain = null!, tabQuery = null!, tabCalibration = null!;

    // 查询 Tab 控件
    private DataGridView dgvRecords = null!;
    private DateTimePicker dtpStart = null!, dtpEnd = null!;
    private TextBox txtQueryProduct = null!;
    private ComboBox cmbQueryOperator = null!;
    private Button btnQuery = null!, btnExportQuery = null!;

    public MainForm()
    {
        _tc = _ctx.TestController;
        InitializeComponent();
        SetupPlot();
        WireEvents();
        UpdateButtonStates();
    }

    #region UI 初始化

    private void InitializeComponent()
    {
        this.Text = "ISO 11820 建筑材料不燃性试验系统";
        this.Size = new Size(1280, 820);
        this.StartPosition = FormStartPosition.CenterScreen;
        this.MinimumSize = new Size(1024, 700);
        this.BackColor = Color.FromArgb(30, 30, 30);
        this.ForeColor = Color.White;

        tabControl = new TabControl
        {
            Dock = DockStyle.Fill,
            Font = new Font("Microsoft YaHei", 10)
        };

        // === Tab 1: 试验主界面 ===
        tabMain = new TabPage("试验控制");
        tabMain.BackColor = Color.FromArgb(30, 30, 30);

        // 顶部温度面板
        var tempPanel = CreateTemperaturePanel();
        // 状态区
        var statusPanel = CreateStatusPanel();
        // 按钮区
        var buttonPanel = CreateButtonPanel();
        // 曲线图
        plotView = new PlotView
        {
            Dock = DockStyle.Fill,
            BackColor = Color.FromArgb(30, 30, 30)
        };
        // 消息日志
        rtbLog = new RichTextBox
        {
            Dock = DockStyle.Fill,
            BackColor = Color.Black,
            ForeColor = Color.White,
            Font = new Font("Consolas", 10),
            ReadOnly = true,
            WordWrap = true
        };

        // 布局
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 160, BackColor = Color.FromArgb(30, 30, 30) };
        topPanel.Controls.Add(tempPanel);
        topPanel.Controls.Add(statusPanel);
        topPanel.Controls.Add(buttonPanel);

        var centerPanel = new Panel { Dock = DockStyle.Fill, BackColor = Color.FromArgb(30, 30, 30) };
        var splitCenter = new SplitContainer
        {
            Dock = DockStyle.Fill,
            Orientation = Orientation.Horizontal,
            SplitterDistance = 320,
            Panel1BackColor = Color.FromArgb(30, 30, 30),
            Panel2BackColor = Color.Black
        };
        splitCenter.Panel1.Controls.Add(plotView);
        splitCenter.Panel2.Controls.Add(rtbLog);

        tabMain.Controls.Add(centerPanel);
        tabMain.Controls.Add(topPanel);
        centerPanel.Controls.Add(splitCenter);

        // === Tab 2: 记录查询 ===
        tabQuery = new TabPage("记录查询");
        tabQuery.BackColor = Color.FromArgb(30, 30, 30);
        BuildQueryTab();

        // === Tab 3: 设备校准 ===
        tabCalibration = new TabPage("设备校准");
        tabCalibration.BackColor = Color.FromArgb(30, 30, 30);
        BuildCalibrationTab();

        tabControl.TabPages.Add(tabMain);
        tabControl.TabPages.Add(tabQuery);
        tabControl.TabPages.Add(tabCalibration);

        this.Controls.Add(tabControl);
    }

    private Panel CreateTemperaturePanel()
    {
        var panel = new Panel { Location = new Point(10, 10), Size = new Size(750, 130), BackColor = Color.FromArgb(30, 30, 30) };

        var labels = new[] { "炉温1", "炉温2", "表面温", "中心温", "校准温" };
        var lblVals = new Label[5];
        var xPos = 10;

        for (int i = 0; i < 5; i++)
        {
            var lbl = new Label
            {
                Text = labels[i],
                ForeColor = Color.Gray,
                Font = new Font("Microsoft YaHei", 9),
                Location = new Point(xPos, 5),
                Size = new Size(130, 20),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lbl);

            lblVals[i] = new Label
            {
                Text = "0.0 °C",
                ForeColor = GetChannelColor(i),
                Font = new Font("Consolas", 26, FontStyle.Bold),
                Location = new Point(xPos, 28),
                Size = new Size(130, 40),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(lblVals[i]);

            // LED 风格边框
            var border = new Panel
            {
                Location = new Point(xPos, 28),
                Size = new Size(130, 40),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.Transparent
            };
            panel.Controls.Add(border);

            // 通道名标签
            var chLabel = new Label
            {
                Text = i switch { 0 => "TF1", 1 => "TF2", 2 => "TS", 3 => "TC", 4 => "TCal" },
                ForeColor = Color.Gray,
                Font = new Font("Consolas", 8),
                Location = new Point(xPos, 72),
                Size = new Size(130, 16),
                TextAlign = ContentAlignment.MiddleCenter
            };
            panel.Controls.Add(chLabel);

            xPos += 145;
        }

        lblTF1Val = lblVals[0]; lblTF2Val = lblVals[1]; lblTSVal = lblVals[2];
        lblTCVal = lblVals[3]; lblTCalVal = lblVals[4];

        return panel;
    }

    private Color GetChannelColor(int i) => i switch
    {
        0 => Color.FromArgb(255, 80, 80),    // 炉温1 - 红
        1 => Color.FromArgb(255, 160, 60),   // 炉温2 - 橙
        2 => Color.FromArgb(80, 200, 80),    // 表面温 - 绿
        3 => Color.FromArgb(80, 180, 255),   // 中心温 - 蓝
        4 => Color.FromArgb(200, 180, 100),  // 校准温 - 金
        _ => Color.White
    };

    private Panel CreateStatusPanel()
    {
        var panel = new Panel { Location = new Point(770, 10), Size = new Size(240, 130), BackColor = Color.FromArgb(45, 45, 45) };

        lblStatus = new Label
        {
            Text = "空闲",
            ForeColor = Color.White,
            Font = new Font("Microsoft YaHei", 14, FontStyle.Bold),
            Location = new Point(10, 10),
            Size = new Size(220, 30),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblTimer = new Label
        {
            Text = "计时: 0 秒",
            ForeColor = Color.FromArgb(0, 200, 200),
            Font = new Font("Consolas", 18, FontStyle.Bold),
            Location = new Point(10, 45),
            Size = new Size(220, 35),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblDrift = new Label
        {
            Text = "温漂: -- °C/10min",
            ForeColor = Color.Gray,
            Font = new Font("Microsoft YaHei", 10),
            Location = new Point(10, 85),
            Size = new Size(220, 20),
            TextAlign = ContentAlignment.MiddleCenter
        };

        lblSample = new Label
        {
            Text = "样品: --",
            ForeColor = Color.Gray,
            Font = new Font("Microsoft YaHei", 9),
            Location = new Point(10, 107),
            Size = new Size(220, 18),
            TextAlign = ContentAlignment.MiddleCenter
        };

        panel.Controls.Add(lblStatus);
        panel.Controls.Add(lblTimer);
        panel.Controls.Add(lblDrift);
        panel.Controls.Add(lblSample);
        return panel;
    }

    private Panel CreateButtonPanel()
    {
        var panel = new Panel { Dock = DockStyle.None, Location = new Point(1020, 10), Size = new Size(220, 130), BackColor = Color.FromArgb(30, 30, 30) };

        btnNewTest = CreateButton("新建试验", new Point(5, 5), Color.FromArgb(60, 120, 200));
        btnStartHeat = CreateButton("开始升温", new Point(5, 42), Color.FromArgb(200, 80, 60));
        btnStopHeat = CreateButton("停止升温", new Point(115, 42), Color.FromArgb(180, 120, 60));
        btnStartRecord = CreateButton("开始记录", new Point(5, 79), Color.FromArgb(40, 160, 80));
        btnStopRecord = CreateButton("停止记录", new Point(115, 79), Color.FromArgb(160, 120, 60));
        btnTestRecord = CreateButton("试验记录", new Point(5, 116), Color.FromArgb(140, 100, 180));
        btnSettings = CreateButton("参数设置", new Point(115, 116), Color.FromArgb(120, 120, 120));

        btnNewTest.Click += (s, e) => OpenNewTestDialog();
        btnStartHeat.Click += (s, e) => { if (_tc.StartHeating()) { _ctx.DaqWorker.Start(); _ctx.DaqWorker.AddMessage("开始升温，系统升温中"); UpdateButtonStates(); } };
        btnStopHeat.Click += (s, e) => { if (_tc.StopHeating()) { _ctx.DaqWorker.Stop(); UpdateButtonStates(); } };
        btnStartRecord.Click += (s, e) => { if (_tc.StartRecording()) { _ctx.DaqWorker.ResetElapsed(); UpdateButtonStates(); } };
        btnStopRecord.Click += (s, e) => { if (_tc.StopRecording()) UpdateButtonStates(); };
        btnTestRecord.Click += (s, e) => OpenTestRecordDialog();
        btnSettings.Click += (s, e) => MessageBox.Show("参数设置功能", "提示");

        panel.Controls.AddRange(new Control[] { btnNewTest, btnStartHeat, btnStopHeat, btnStartRecord, btnStopRecord, btnTestRecord, btnSettings });
        return panel;
    }

    private Button CreateButton(string text, Point loc, Color backColor)
    {
        return new Button
        {
            Text = text,
            Location = loc,
            Size = new Size(105, 30),
            Font = new Font("Microsoft YaHei", 9),
            BackColor = backColor,
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
    }

    #endregion

    #region 曲线图

    private void SetupPlot()
    {
        plotModel = new PlotModel { Title = "温度曲线", TextColor = OxyColors.White, PlotAreaBorderColor = OxyColors.Gray };
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Bottom,
            Title = "时间 (秒)",
            Minimum = 0,
            Maximum = 600,
            TextColor = OxyColors.White,
            TitleColor = OxyColors.White,
            AxislineColor = OxyColors.Gray,
            TicklineColor = OxyColors.Gray
        });
        plotModel.Axes.Add(new LinearAxis
        {
            Position = AxisPosition.Left,
            Title = "温度 (°C)",
            Minimum = 0,
            Maximum = 800,
            TextColor = OxyColors.White,
            TitleColor = OxyColors.White,
            AxislineColor = OxyColors.Gray,
            TicklineColor = OxyColors.Gray
        });

        seriesTF1 = CreateLineSeries("炉温1", OxyColors.Red);
        seriesTF2 = CreateLineSeries("炉温2", OxyColors.Orange);
        seriesTS = CreateLineSeries("表面温", OxyColors.LimeGreen);
        seriesTC = CreateLineSeries("中心温", OxyColors.SkyBlue);

        plotModel.Series.Add(seriesTF1);
        plotModel.Series.Add(seriesTF2);
        plotModel.Series.Add(seriesTS);
        plotModel.Series.Add(seriesTC);

        plotView.Model = plotModel;
    }

    private LineSeries CreateLineSeries(string title, OxyColor color)
    {
        return new LineSeries
        {
            Title = title,
            Color = color,
            StrokeThickness = 1.5,
            MarkerType = MarkerType.None
        };
    }

    #endregion

    #region 查询 Tab

    private void BuildQueryTab()
    {
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 50, BackColor = Color.FromArgb(40, 40, 40) };

        var lblStart = new Label { Text = "开始日期:", ForeColor = Color.White, Location = new Point(10, 15), Size = new Size(70, 22) };
        dtpStart = new DateTimePicker { Location = new Point(80, 12), Size = new Size(130, 24), Format = DateTimePickerFormat.Short };
        var lblEnd = new Label { Text = "结束日期:", ForeColor = Color.White, Location = new Point(220, 15), Size = new Size(70, 22) };
        dtpEnd = new DateTimePicker { Location = new Point(290, 12), Size = new Size(130, 24), Format = DateTimePickerFormat.Short };
        var lblProduct = new Label { Text = "样品编号:", ForeColor = Color.White, Location = new Point(430, 15), Size = new Size(70, 22) };
        txtQueryProduct = new TextBox { Location = new Point(500, 12), Size = new Size(100, 24) };
        cmbQueryOperator = new ComboBox { Location = new Point(610, 12), Size = new Size(100, 24), DropDownStyle = ComboBoxStyle.DropDownList };
        btnQuery = new Button { Text = "查询", Location = new Point(720, 10), Size = new Size(80, 30), BackColor = Color.FromArgb(60, 120, 200), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnExportQuery = new Button { Text = "导出Excel", Location = new Point(810, 10), Size = new Size(90, 30), BackColor = Color.FromArgb(40, 160, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };

        topPanel.Controls.AddRange(new Control[] { lblStart, dtpStart, lblEnd, dtpEnd, lblProduct, txtQueryProduct, cmbQueryOperator, btnQuery, btnExportQuery });

        dgvRecords = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            GridColor = Color.Gray,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect
        };

        btnQuery.Click += (s, e) => ExecuteQuery();
        btnExportQuery.Click += (s, e) => ExportQueryResults();
        dgvRecords.CellDoubleClick += (s, e) => ViewRecordDetail();

        tabQuery.Controls.Add(dgvRecords);
        tabQuery.Controls.Add(topPanel);

        // 加载操作员列表
        var ops = _ctx.Db.GetAllOperators();
        cmbQueryOperator.Items.Add("全部");
        foreach (var op in ops) cmbQueryOperator.Items.Add(op.Username);
        cmbQueryOperator.SelectedIndex = 0;
    }

    private void ExecuteQuery()
    {
        string? productId = string.IsNullOrWhiteSpace(txtQueryProduct.Text) ? null : txtQueryProduct.Text;
        string? opName = cmbQueryOperator.SelectedIndex <= 0 ? null : cmbQueryOperator.SelectedItem?.ToString();
        string? startDate = dtpStart.Value.ToString("yyyy-MM-dd");
        string? endDate = dtpEnd.Value.ToString("yyyy-MM-dd");

        var records = _ctx.Db.QueryTestMasters(productId, opName, startDate, endDate);

        dgvRecords.DataSource = records.Select(r => new
        {
            r.ProductId,
            r.TestId,
            r.TestDate,
            r.Operator,
            r.PreWeight,
            r.PostWeight,
            失重率 = $"{r.LostWeightPer:F2}%",
            综合温升 = $"{r.DeltaTf:F1}°C",
            时长 = $"{r.TotalTestTime}s",
            r.Flag
        }).ToList();
    }

    private void ExportQueryResults()
    {
        if (dgvRecords.Rows.Count == 0)
        {
            MessageBox.Show("没有可导出的数据", "提示");
            return;
        }
        // 简单导出为 CSV
        using var sfd = new SaveFileDialog { Filter = "CSV文件|*.csv", FileName = $"查询导出_{DateTime.Now:yyyyMMddHHmmss}.csv" };
        if (sfd.ShowDialog() == DialogResult.OK)
        {
            var sb = new System.Text.StringBuilder();
            // 表头
            var headers = dgvRecords.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText);
            sb.AppendLine(string.Join(",", headers));
            // 数据
            foreach (DataGridViewRow row in dgvRecords.Rows)
            {
                var cells = row.Cells.Cast<DataGridViewCell>().Select(c => c.Value?.ToString() ?? "");
                sb.AppendLine(string.Join(",", cells));
            }
            File.WriteAllText(sfd.FileName, sb.ToString(), System.Text.Encoding.UTF8);
            MessageBox.Show($"导出成功: {sfd.FileName}", "提示");
        }
    }

    private void ViewRecordDetail()
    {
        if (dgvRecords.CurrentRow?.DataBoundItem == null) return;
        // 获取 productId 和 testId
        dynamic item = dgvRecords.CurrentRow.DataBoundItem;
        string pid = item.ProductId;
        string tid = item.TestId;
        var tm = _ctx.Db.GetTestMaster(pid, tid);
        if (tm == null) return;

        var detail = $"样品编号: {tm.ProductId}\n试验标识: {tm.TestId}\n日期: {tm.TestDate}\n" +
                     $"操作员: {tm.Operator}\n环境温度: {tm.EnvTemp:F1}°C\n环境湿度: {tm.EnvHumidity:F1}%\n" +
                     $"试验前质量: {tm.PreWeight:F2}g\n试验后质量: {tm.PostWeight:F2}g\n" +
                     $"失重率: {tm.LostWeightPer:F2}%\n综合温升: {tm.DeltaTf:F1}°C\n" +
                     $"炉温1温升: {tm.DeltaTf1:F1}°C\n炉温2温升: {tm.DeltaTf2:F1}°C\n" +
                     $"表面温升: {tm.DeltaTs:F1}°C\n中心温升: {tm.DeltaTc:F1}°C\n" +
                     $"火焰持续时间: {tm.FlameDuration}s\n试验时长: {tm.TotalTestTime}s\n备注: {tm.Remark}";
        MessageBox.Show(detail, "试验详情", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    #endregion

    #region 校准 Tab

    private Label lblCalTemp = null!;
    private TextBox txtCalRefTemp = null!;
    private Button btnCalRecord = null!;
    private DataGridView dgvCalRecords = null!;

    private void BuildCalibrationTab()
    {
        var topPanel = new Panel { Dock = DockStyle.Top, Height = 80, BackColor = Color.FromArgb(40, 40, 40) };

        lblCalTemp = new Label
        {
            Text = "当前校准温: 0.0 °C",
            ForeColor = Color.FromArgb(200, 180, 100),
            Font = new Font("Consolas", 20, FontStyle.Bold),
            Location = new Point(10, 15),
            Size = new Size(300, 40),
            TextAlign = ContentAlignment.MiddleCenter
        };

        var lblRef = new Label { Text = "标准温度(°C):", ForeColor = Color.White, Location = new Point(320, 20), Size = new Size(100, 24) };
        txtCalRefTemp = new TextBox { Location = new Point(420, 18), Size = new Size(80, 24), Text = "750" };
        btnCalRecord = new Button
        {
            Text = "记录校准",
            Location = new Point(520, 15),
            Size = new Size(100, 32),
            BackColor = Color.FromArgb(60, 120, 200),
            ForeColor = Color.White,
            FlatStyle = FlatStyle.Flat
        };
        btnCalRecord.Click += (s, e) => RecordCalibration();

        topPanel.Controls.AddRange(new Control[] { lblCalTemp, lblRef, txtCalRefTemp, btnCalRecord });

        dgvCalRecords = new DataGridView
        {
            Dock = DockStyle.Fill,
            BackgroundColor = Color.FromArgb(30, 30, 30),
            ForeColor = Color.White,
            GridColor = Color.Gray,
            AllowUserToAddRows = false,
            ReadOnly = true,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill
        };

        tabCalibration.Controls.Add(dgvCalRecords);
        tabCalibration.Controls.Add(topPanel);

        LoadCalibrationRecords();
    }

    private void RecordCalibration()
    {
        if (!double.TryParse(txtCalRefTemp.Text, out double refTemp)) return;
        double measured = _ctx.DaqWorker.Temperatures["TCal"];
        double deviation = measured - refTemp;

        _ctx.Db.InsertCalibrationRecord(new CalibrationRecord
        {
            CalibrationDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Operator = _ctx.CurrentOperator,
            ReferenceTemp = refTemp,
            MeasuredTemp = measured,
            Deviation = deviation
        });

        LoadCalibrationRecords();
        MessageBox.Show($"校准记录已保存\n标准温度: {refTemp:F1}°C\n实测温度: {measured:F1}°C\n偏差: {deviation:F1}°C", "校准完成");
    }

    private void LoadCalibrationRecords()
    {
        var records = _ctx.Db.GetCalibrationRecords();
        dgvCalRecords.DataSource = records.Select(r => new
        {
            r.Id,
            r.CalibrationDate,
            r.Operator,
            r.ReferenceTemp,
            r.MeasuredTemp,
            r.Deviation
        }).ToList();
    }

    #endregion

    #region 事件处理

    private void WireEvents()
    {
        _ctx.DaqWorker.DataBroadcast += OnDataBroadcast;
        _tc.StateChanged += OnStateChanged;
    }

    private void OnDataBroadcast(object? sender, DataBroadcastEventArgs e)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(() => OnDataBroadcast(sender, e));
            return;
        }

        // 更新温度显示
        var temps = e.Temperatures;
        lblTF1Val.Text = $"{temps["TF1"]:F1} °C";
        lblTF2Val.Text = $"{temps["TF2"]:F1} °C";
        lblTSVal.Text = $"{temps["TS"]:F1} °C";
        lblTCVal.Text = $"{temps["TC"]:F1} °C";
        lblTCalVal.Text = $"{temps["TCal"]:F1} °C";
        lblCalTemp.Text = $"当前校准温: {temps["TCal"]:F1} °C";

        // 更新计时器
        lblTimer.Text = $"计时: {e.ElapsedSeconds} 秒";

        // 更新温漂
        if (!double.IsNaN(e.Drift))
            lblDrift.Text = $"温漂: {e.Drift:F2} °C/10min";

        // 更新样品信息
        if (_tc.CurrentTest != null)
            lblSample.Text = $"样品: {_tc.CurrentTest.ProductId}";

        // 更新曲线图
        _timeCounter++;
        _timePoints.Add(_timeCounter);
        if (_timePoints.Count > 600) _timePoints.RemoveAt(0);

        seriesTF1.Points.Add(new DataPoint(_timeCounter, temps["TF1"]));
        seriesTF2.Points.Add(new DataPoint(_timeCounter, temps["TF2"]));
        seriesTS.Points.Add(new DataPoint(_timeCounter, temps["TS"]));
        seriesTC.Points.Add(new DataPoint(_timeCounter, temps["TC"]));

        // 滚动 X 轴
        if (_timeCounter > 600)
        {
            plotModel.Axes[0].Minimum = _timeCounter - 600;
            plotModel.Axes[0].Maximum = _timeCounter;
        }

        // 限制点数，防止内存泄漏
        if (seriesTF1.Points.Count > 1200)
        {
            foreach (var series in new[] { seriesTF1, seriesTF2, seriesTS, seriesTC })
            {
                while (series.Points.Count > 600)
                    series.Points.RemoveAt(0);
            }
        }

        plotModel.InvalidatePlot(true);

        // 更新消息日志
        foreach (var msg in e.Messages)
        {
            Color color = msg.Message.Contains("终止") ? Color.Yellow :
                          msg.Message.Contains("错误") ? Color.Red : Color.White;
            rtbLog.SelectionColor = color;
            rtbLog.AppendText($"{msg.Time}  {msg.Message}\n");
            rtbLog.ScrollToCaret();
        }
    }

    private void OnStateChanged(object? sender, string state)
    {
        if (this.InvokeRequired)
        {
            this.Invoke(() => OnStateChanged(sender, state));
            return;
        }

        lblStatus.Text = state switch
        {
            "Idle" => "空闲",
            "Preparing" => "升温中",
            "Ready" => "就绪",
            "Recording" => "记录中",
            "Complete" => "完成",
            _ => state
        };

        UpdateButtonStates();
    }

    #endregion

    #region 按钮状态 + 对话框

    private void UpdateButtonStates()
    {
        var state = _tc.State;
        bool hasUnSaved = _tc.HasUnSavedCompleteTest();
        bool hasActiveTest = _tc.CurrentTest != null;

        btnNewTest.Enabled = state == TestState.Idle ||
            (state == TestState.Preparing && !hasActiveTest) ||
            (state == TestState.Complete && !hasUnSaved);

        btnStartHeat.Enabled = state == TestState.Idle;
        btnStopHeat.Enabled = state == TestState.Preparing || state == TestState.Ready || state == TestState.Complete;
        btnStartRecord.Enabled = state == TestState.Ready && !hasUnSaved;
        btnStopRecord.Enabled = state == TestState.Recording;
        btnTestRecord.Enabled = state == TestState.Complete && hasUnSaved;
        btnSettings.Enabled = state != TestState.Recording;
    }

    private void OpenNewTestDialog()
    {
        using var dlg = new NewTestForm();
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _tc.CreateTest(dlg.TestMaster!, dlg.ProductMaster!);
            if (!_ctx.DaqWorker.Temperatures.ContainsKey("TF1") ||
                _ctx.DaqWorker.Temperatures["TF1"] < 100)
            {
                _ctx.DaqWorker.Start();
            }
            UpdateButtonStates();
        }
    }

    private void OpenTestRecordDialog()
    {
        if (_tc.CurrentTest == null) return;
        using var dlg = new TestRecordForm(_tc.CurrentTest);
        if (dlg.ShowDialog() == DialogResult.OK)
        {
            _tc.SaveTestRecord(dlg.PostWeight, dlg.HasFlame ? 1 : 0,
                dlg.FlameStartTime, dlg.FlameDuration, dlg.Remark);

            // 导出文件
            var tm = _tc.CurrentTest;
            var tempData = _tc.TemperatureHistory;
            try
            {
                _ctx.ExportService.ExportCsv(tm, tempData);
                _ctx.ExportService.ExportExcel(tm, tempData);
                if (_ctx.Configuration.GetValue<bool>("Report:EnablePdfExport"))
                    _ctx.ExportService.ExportPdf(tm, tempData);
                MessageBox.Show("试验记录已保存，报告已生成。", "完成", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"导出报告失败: {ex.Message}", "错误", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            _tc.ClearCurrentTest();
            UpdateButtonStates();
        }
    }

    #endregion

    protected override void OnFormClosing(FormClosingEventArgs e)
    {
        _ctx.DaqWorker.Stop();
        _ctx.DaqWorker.Dispose();
        base.OnFormClosing(e);
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: MainForm references NewTestForm and TestRecordForm which don't exist yet — but they will be created in Tasks 11-12. If build fails due to missing types, proceed to next tasks.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add main form with temperature display, chart, message log"
```

---

### Task 11: 子窗体 — NewTestForm

**Files:**
- Create: `ISO_System/ISO11820/Forms/NewTestForm.cs`

- [ ] **Step 1: NewTestForm.cs**

Write `ISO_System/ISO11820/Forms/NewTestForm.cs`:
```csharp
using ISO11820.Global;
using ISO11820.Models;

namespace ISO11820.Forms;

public partial class NewTestForm : Form
{
    public TestMaster? TestMaster { get; private set; }
    public ProductMaster? ProductMaster { get; private set; }

    private TextBox txtProductId = null!, txtTestId = null!, txtProductName = null!;
    private TextBox txtSpecification = null!, txtHeight = null!, txtDiameter = null!;
    private TextBox txtEnvTemp = null!, txtEnvHumidity = null!, txtPreWeight = null!;
    private ComboBox cmbDurationMode = null!;
    private NumericUpDown nudTargetDuration = null!;
    private Label lblOperator = null!, lblApparatus = null!;
    private Button btnOK = null!, btnCancel = null!;

    public NewTestForm()
    {
        InitializeComponent();
        LoadApparatusInfo();
    }

    private void InitializeComponent()
    {
        this.Text = "新建试验";
        this.Size = new Size(480, 520);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(240, 240, 240);

        int y = 15;
        int leftX = 130;

        // 样品信息
        AddLabel("样品编号:", 15, y); txtProductId = AddTextBox(leftX, y, 280); y += 32;
        AddLabel("试验标识:", 15, y); txtTestId = AddTextBox(leftX, y, 280); y += 32;
        AddLabel("样品名称:", 15, y); txtProductName = AddTextBox(leftX, y, 280); y += 32;
        AddLabel("规格:", 15, y); txtSpecification = AddTextBox(leftX, y, 280); y += 32;
        AddLabel("高度 (mm):", 15, y); txtHeight = AddTextBox(leftX, y, 100); y += 32;
        AddLabel("直径 (mm):", 15, y); txtDiameter = AddTextBox(leftX, y, 100); y += 32;

        // 环境信息
        AddLabel("环境温度 (°C):", 15, y); txtEnvTemp = AddTextBox(leftX, y, 100); txtEnvTemp.Text = "25.0"; y += 32;
        AddLabel("环境湿度 (%):", 15, y); txtEnvHumidity = AddTextBox(leftX, y, 100); txtEnvHumidity.Text = "50.0"; y += 32;

        // 试验参数
        AddLabel("试验前质量 (g):", 15, y); txtPreWeight = AddTextBox(leftX, y, 100); txtPreWeight.Text = "50.0"; y += 32;

        AddLabel("时长模式:", 15, y);
        cmbDurationMode = new ComboBox { Location = new Point(leftX, y - 2), Size = new Size(120, 24), DropDownStyle = ComboBoxStyle.DropDownList };
        cmbDurationMode.Items.AddRange(new[] { "标准 60 分钟", "自定义" });
        cmbDurationMode.SelectedIndex = 0;
        this.Controls.Add(cmbDurationMode);
        y += 32;

        AddLabel("自定义时长 (秒):", 15, y);
        nudTargetDuration = new NumericUpDown { Location = new Point(leftX, y - 2), Size = new Size(100, 24), Minimum = 60, Maximum = 7200, Value = 3600, Enabled = false };
        this.Controls.Add(nudTargetDuration);
        cmbDurationMode.SelectedIndexChanged += (s, e) => nudTargetDuration.Enabled = cmbDurationMode.SelectedIndex == 1;
        y += 32;

        // 操作员和设备信息（自动填入）
        AddLabel("操作员:", 15, y);
        lblOperator = new Label { Text = AppContext.Instance.CurrentOperator, Location = new Point(leftX, y), Size = new Size(200, 22), Font = new Font("Microsoft YaHei", 10, FontStyle.Bold) };
        this.Controls.Add(lblOperator);
        y += 32;

        AddLabel("设备信息:", 15, y);
        lblApparatus = new Label { Text = "--", Location = new Point(leftX, y), Size = new Size(280, 22), Font = new Font("Microsoft YaHei", 9) };
        this.Controls.Add(lblApparatus);
        y += 42;

        // 按钮
        btnOK = new Button { Text = "创建试验", Location = new Point(120, y), Size = new Size(110, 38), BackColor = Color.FromArgb(0, 122, 204), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.Click += BtnOK_Click;
        btnCancel = new Button { Text = "取消", Location = new Point(250, y), Size = new Size(100, 38), FlatStyle = FlatStyle.Flat };
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        this.Controls.Add(btnOK);
        this.Controls.Add(btnCancel);
    }

    private void AddLabel(string text, int x, int y)
    {
        this.Controls.Add(new Label
        {
            Text = text,
            Location = new Point(x, y),
            Size = new Size(110, 22),
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Microsoft YaHei", 9)
        });
    }

    private TextBox AddTextBox(int x, int y, int width)
    {
        var tb = new TextBox { Location = new Point(x, y), Size = new Size(width, 24), Font = new Font("Microsoft YaHei", 9) };
        this.Controls.Add(tb);
        return tb;
    }

    private void LoadApparatusInfo()
    {
        var app = AppContext.Instance.Db.GetApparatus("ISO11820-001");
        if (app != null)
            lblApparatus.Text = $"{app.ApparatusName} ({app.ApparatusId})";
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtProductId.Text) || string.IsNullOrWhiteSpace(txtTestId.Text))
        {
            MessageBox.Show("样品编号和试验标识为必填项", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var app = AppContext.Instance.Db.GetApparatus("ISO11820-001");

        TestMaster = new TestMaster
        {
            ProductId = txtProductId.Text.Trim(),
            TestId = txtTestId.Text.Trim(),
            TestDate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Operator = AppContext.Instance.CurrentOperator,
            EnvTemp = double.TryParse(txtEnvTemp.Text, out double et) ? et : 25.0,
            EnvHumidity = double.TryParse(txtEnvHumidity.Text, out double eh) ? eh : 50.0,
            PreWeight = double.TryParse(txtPreWeight.Text, out double pw) ? pw : 50.0,
            DurationMode = cmbDurationMode.SelectedIndex == 0 ? "Standard" : "Fixed",
            TargetDuration = (int)nudTargetDuration.Value,
            ApparatusId = app?.ApparatusId ?? "ISO11820-001",
            ApparatusName = app?.ApparatusName ?? "不燃性试验炉",
            ApparatusCalibrationDate = app?.CalibrationDate ?? "",
            ConstPower = app?.ConstPower ?? 2048,
            Flag = ""
        };

        ProductMaster = new ProductMaster
        {
            ProductId = txtProductId.Text.Trim(),
            TestId = txtTestId.Text.Trim(),
            ProductName = txtProductName.Text.Trim(),
            Specification = txtSpecification.Text.Trim(),
            Height = double.TryParse(txtHeight.Text, out double h) ? h : 0,
            Diameter = double.TryParse(txtDiameter.Text, out double d) ? d : 0
        };

        this.DialogResult = DialogResult.OK;
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds or fails only on TestRecordForm reference (Task 12).

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add new test dialog form"
```

---

### Task 12: 子窗体 — TestRecordForm（试验现象记录）

**Files:**
- Create: `ISO_System/ISO11820/Forms/TestRecordForm.cs`

- [ ] **Step 1: TestRecordForm.cs**

Write `ISO_System/ISO11820/Forms/TestRecordForm.cs`:
```csharp
using ISO11820.Models;

namespace ISO11820.Forms;

public partial class TestRecordForm : Form
{
    public double PostWeight { get; private set; }
    public bool HasFlame { get; private set; }
    public int FlameStartTime { get; private set; }
    public int FlameDuration { get; private set; }
    public string Remark { get; private set; } = string.Empty;

    private CheckBox chkFlame = null!;
    private NumericUpDown nudFlameStart = null!, nudFlameDuration = null!;
    private TextBox txtPostWeight = null!, txtRemark = null!;
    private Button btnOK = null!, btnCancel = null!;

    public TestRecordForm(TestMaster tm)
    {
        InitializeComponent();
        this.Text = $"试验记录 — {tm.ProductId} / {tm.TestId}";

        // 预填初始质量
        var lblPreWeight = new Label
        {
            Text = $"试验前质量: {tm.PreWeight:F2} g",
            Font = new Font("Microsoft YaHei", 10, FontStyle.Bold),
            Location = new Point(15, 15),
            Size = new Size(250, 24)
        };
        this.Controls.Add(lblPreWeight);
    }

    private void InitializeComponent()
    {
        this.Size = new Size(440, 380);
        this.StartPosition = FormStartPosition.CenterParent;
        this.FormBorderStyle = FormBorderStyle.FixedDialog;
        this.MaximizeBox = false;
        this.BackColor = Color.FromArgb(240, 240, 240);

        int y = 50;
        int leftX = 150;

        // 试验后质量
        var lblPostWeight = new Label { Text = "试验后质量 (g):", Location = new Point(15, y), Size = new Size(130, 24), TextAlign = ContentAlignment.MiddleRight, Font = new Font("Microsoft YaHei", 9) };
        txtPostWeight = new TextBox { Location = new Point(leftX, y), Size = new Size(100, 24), Font = new Font("Microsoft YaHei", 9) };
        this.Controls.Add(lblPostWeight);
        this.Controls.Add(txtPostWeight);
        y += 38;

        // 火焰
        chkFlame = new CheckBox
        {
            Text = "是否出现持续火焰",
            Location = new Point(leftX, y),
            Size = new Size(160, 24),
            Font = new Font("Microsoft YaHei", 9),
            Checked = false
        };
        chkFlame.CheckedChanged += (s, e) =>
        {
            nudFlameStart.Enabled = nudFlameDuration.Enabled = chkFlame.Checked;
        };
        this.Controls.Add(chkFlame);
        y += 32;

        var lblFlameStart = new Label { Text = "火焰发生时刻 (秒):", Location = new Point(15, y), Size = new Size(130, 24), TextAlign = ContentAlignment.MiddleRight, Font = new Font("Microsoft YaHei", 9) };
        nudFlameStart = new NumericUpDown { Location = new Point(leftX, y), Size = new Size(100, 24), Minimum = 0, Maximum = 7200, Enabled = false };
        this.Controls.Add(lblFlameStart);
        this.Controls.Add(nudFlameStart);
        y += 32;

        var lblFlameDuration = new Label { Text = "火焰持续时间 (秒):", Location = new Point(15, y), Size = new Size(130, 24), TextAlign = ContentAlignment.MiddleRight, Font = new Font("Microsoft YaHei", 9) };
        nudFlameDuration = new NumericUpDown { Location = new Point(leftX, y), Size = new Size(100, 24), Minimum = 0, Maximum = 7200, Enabled = false };
        this.Controls.Add(lblFlameDuration);
        this.Controls.Add(nudFlameDuration);
        y += 38;

        // 备注
        var lblRemark = new Label { Text = "备注:", Location = new Point(15, y), Size = new Size(130, 24), TextAlign = ContentAlignment.MiddleRight, Font = new Font("Microsoft YaHei", 9) };
        txtRemark = new TextBox { Location = new Point(leftX, y), Size = new Size(250, 60), Multiline = true, Font = new Font("Microsoft YaHei", 9) };
        this.Controls.Add(lblRemark);
        this.Controls.Add(txtRemark);
        y += 75;

        // 按钮
        btnOK = new Button { Text = "保存", Location = new Point(110, y), Size = new Size(100, 38), BackColor = Color.FromArgb(40, 160, 80), ForeColor = Color.White, FlatStyle = FlatStyle.Flat };
        btnOK.FlatAppearance.BorderSize = 0;
        btnOK.Click += BtnOK_Click;
        btnCancel = new Button { Text = "取消", Location = new Point(230, y), Size = new Size(100, 38), FlatStyle = FlatStyle.Flat };
        btnCancel.Click += (s, e) => this.DialogResult = DialogResult.Cancel;

        this.Controls.Add(btnOK);
        this.Controls.Add(btnCancel);
    }

    private void BtnOK_Click(object? sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txtPostWeight.Text) || !double.TryParse(txtPostWeight.Text, out double pw))
        {
            MessageBox.Show("请输入有效的试验后质量", "验证失败", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        PostWeight = pw;
        HasFlame = chkFlame.Checked;
        FlameStartTime = (int)nudFlameStart.Value;
        FlameDuration = (int)nudFlameDuration.Value;
        Remark = txtRemark.Text;

        this.DialogResult = DialogResult.OK;
    }
}
```

- [ ] **Step 2: 验证编译**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds — all types now exist.

- [ ] **Step 3: 提交**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: add test record dialog form"
```

---

### Task 13: 集成验证 — 编译并启动

**Files:** None new —验证现有代码可编译和运行。

- [ ] **Step 1: 编译整个项目**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet build
```

Expected: Build succeeds with no errors.

- [ ] **Step 2: 修复编译问题（如有）**

If there are compilation errors, fix them:
- Check all `using` statements are correct
- Verify all referenced types exist
- Ensure namespace consistency (`ISO11820.Models`, `ISO11820.Data`, etc.)

- [ ] **Step 3: 运行集成测试（启动 -> 登录 -> 创建试验 -> 升温 -> 曲线图）**

Run:
```bash
cd "d:/code/ideaPrograms/ISO_System/ISO11820"
dotnet run
```

Manual verification steps:
1. Login form appears → login as admin/123456
2. Main form appears with all tabs
3. Click "新建试验" → fill in details → click "创建试验"
4. Click "开始升温" → watch temperatures rise on chart
5. Wait for "Ready" state → click "开始记录"
6. Wait a few seconds → click "停止记录"
7. Click "试验记录" → fill in post-weight → save
8. Verify CSV/Excel/PDF files are generated
9. Switch to "记录查询" tab → verify record appears
10. Switch to "设备校准" tab → verify calibration temperature display

- [ ] **Step 4: 提交最终代码**

```bash
cd "d:/code/ideaPrograms/ISO_System"
git add -A
git commit -m "feat: complete ISO11820 simulation system - all features integrated"
```

---

## 补充修复 / 已知注意事项

1. **MainForm 中的 DaqWorker 秒计数器同步**：当前 `DaqWorker` 在 `OnTick` 中每 800ms 触发。秒级逻辑（记录温度点、计时器递增）需要在 1000ms 累计后再调用 `TestController.OnSecondElapsed()`。需在 DaqWorker 或 TestController 的集成处增加这部分桥接代码。

2. **TestController.DoWork 调用**：`DaqWorker.OnTick()` 应调用 `TestController.DoWork()`，`DoWork()` 内部的终止条件检查需要 `ElapsedSeconds`，这个值由 `TestController.OnSecondElapsed()` 维护。

3. **试验记录保存后不自动隐藏**：`MainForm.OpenTestRecordDialog()` 中 `SaveTestRecord` 后调用 `ClearCurrentTest()`，状态回到 Preparing，按钮也更新正确。

4. **EPPlus 许可证**：`ExportService` 中设置 `ExcelPackage.LicenseContext = LicenseContext.NonCommercial`。

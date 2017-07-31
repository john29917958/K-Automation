K-Automation
===
The framework for the NCU Korat library.

# Requirements
Here lists all requirements for the Korat framework.

## Answering Questions
1. What kind of applications do you build?
   Ans: Data crawling automation agent with UI automation technique
2. What kind of data sources do you require?
   Ans: Converting CSV, Excel to local file or to remote DB (oracle)
3. What kind of user interface do you use?
   Ans: None. For central Korat manager, yes, a web UI.

## Deal With Potential Change
- [Deal with image change](https://drive.google.com/open?id=0B3oKyZQ9BX2aZWQ5ZktCRHdQekU)
- [Deal with behavior change](https://drive.google.com/open?id=0B3oKyZQ9BX2aZWQ5ZktCRHdQekU)
- Both of them above should take "extension" into account. For example:
    - New behavior "logout" is added to the target application, then in "`xxxBehaviors`", a new method called "`LogOut()`" should be added
    - Logout requires the click of the logout button, so new image named "logout.png" should be added as well
- Configuration change
    - Value change (ex: someday the login password is modified by the user, so the password used by Korat AP should be updated as well.)
    - Entries of configuration file may differs from each Korat AP. For example:
        - Korat AP1 requires 1 database table name, while the Korat AP2 requires 3 database table names

## Enhanced Debugging
- Logging:
    - Logs to local file
    - Logs to Korat manager in the future
- Logs which step failed (Currently I log the `StackTrace` in TSMC)
- Logs screenshots of target application once failed. And combines these screenshots with the `StackTrace`
- Is logging of video (korat-log.mp4?) files possible? or reasonable? And links the `StackTrace` to the specific point of the `korat-log.mp4`
- Notifies developer while script failed

## Commonly Used Functions / Behaviors
### General
- Only one Korat instance should be executed at the same time

### TSMC Specific
- Always dumps and converts data file and then store it to file system or database
- Avoid storing duplicate data
- Usually requires username and password to login into target application

## Powers Up the Korat Ecosystem
- Recognition APIs and convenient mouse, keyboard APIs I've added in TSMC
    - `Korat.RecognizeSingle();`
    - `Korat.RapidInput();`
    - `Korat.SolidColorRecognition();`
    - `Korat.KeyDown();`
    - `Korat.KeyUp();`
    - `string str = Os.CopySelectedText();`
    - `Os.OpenApp("chrome.exe");`
    - `Browser.Launch("https://www.google.com.tw");`
    - `Browser.OpenJavaScriptConsole();`
    - `Dropdown.SelectEach((index, visibleIndex) => { // handler function. });`
    - Etc...
- Generation of temporate image with unique file name, as well as the deletion of these temporate image files after use: `Korat.CropAsTempImage(10, 35, 100, 150);`
- A brand new idea: The `Component`! Something like `Korat.Recognize(obj).SearchRight(template).Click();` where the `Korat.Recognize(obj)` returns a `Component` object with APIs like `Click()`, `RightClick()`, `SearchRight(vlp)`, and so on...
- The `RecognitionArgs` object that builds VLP / IRP string
  ```
  RecognitionArgs args = new RecognitionArgs()
      .Template("template.png", "TM:ccoeffnormed:0.89")
      .Anchor("anchor.png", 5, 10, "TM:ccoeffnormed:0.95")
      .Region(100, 150, 300, 600);
  
  Korat.Recognize(args);
  ```
  Or something like:
  ```
  Component component = Korat.Recognizer
      .Template("template.png", "TM:ccoeffnormed:0.89")
      .Anchor("anchor.png", 5, 10, "TM:ccoeffnormed:0.95")
      .Region(100, 150, 300, 600)
      .Recognize();
  ```

## Potential Growth As Korat Spreads Out in TSMC
- Integrate with central Korat manager server. Korat manager is a web system that:
    -  Accessible anytime, anywhere on any device in TSMC
    -  With clean UI
    -  To start / restart Korat APs
    -  To view status of Korat APs
    -  To scheduling Korat APs
    -  Auto-deploys productional Korat APs
    -  Auto-balances the load of VMs that runs Korat APs (multiple Korat APs in single VM)
    -  Auto-alarming and notifies the person in charge of Korat team (It should be Tzu-Chao, Wang only I think... = =)
    -  Present log history gracefully on the web UI

## Not Sure
- ~~Revertible script execution (revert to specific step once certain step failed)~~ => Okay, this should be unnecessary. However, Korat AP should be restarted once failed. And maybe, a retry count is required.
- Strongly limit the way that user writes Korat AP. In a single step:
    - Pre-check
    - Operation / Behavior
    - Post-check

## Tips to Keep In Mind
- Simplicity: A well designed framework can be taught to a new developer in a few days at most.
- Clarity: The behavioral aspects of the framework must be encapsulated.
- Boundaries: It's a framework, it's not the application itself.
- Expandability: Consistant interfaces, easy to add new classes that extends from native framework classes, hooks.
- Not only figure out framework classes, also presumes how developer uses and extends framework classes.

# Architecture Design
Version 1. **!!Out-Dated!!**
![Imgur](http://i.imgur.com/jyTqW2G.png)

# Problems
Here lists all problems to be solved in conclusion.
1. [The core problem: Behavior change](#design-behavior-change)
2. [Image change](#design-image-change)
3. [How user uses logger? Design of loggers](#design-logging)
4. [How to force user to use Korat Framework in the right way](#design-how-to-force-user-use-korat-framework-in-the-right-way)
5. [Configuration file design](#design-configuration-file)

# Design: Behavior Change
1. Distribution: Open JavaScript console on Chrome v.s. IE v.s. Firefox v.s. Safari
2. Target application evolved to next release version:
   Log in into Facebook 1.0
   ![Imgur](http://i.imgur.com/gPyZnwu.jpg)
   v.s. log in into Facebook now
   ![Imgur](http://i.imgur.com/7ClLLpF.png)
3. New feature included in target application. For example, in early version of Facebook, we cannot send emoji in messenger, but now, we can!

## The Most 機歪 Maintanance Issue I Can Think Of
Scope: Target application evolved to next release version.
I... don't know if this scenario would happen or not. So I just record it here.

### Scenario
For a TSMC web application, there is a behavior class named `TsmcWebApp` with method `Login(string username, string password)`. Someday in the future, the web UI updated. To log in into the web system, not only username and password are required, the user needs to input "Fab name". So the API `Login(string username, string password)` should be updated to `Login(string username, string password, string fabName)`.

### The Problem
Considering 1000 Korat APs use `TsmcWebApp.Login()`. Is it possible to replace `TsmcWebApp.Login(string username, string password)` with `TsmcWebApp.Login(string username, string password, string fabName)` without modifying these 1000 Korat APs?

### Solution: Reads Every OS I/O Events From File
This solution fails if the parameters to be passed to `TsmcWebApp.Login()` are dynamically generated from the previous step.
Solution detail is, encapsulates all parameters into an `io-res.txt`:
```
Login "Jimmy5566{Key-Tab}5566JimmyPassword{Key-Tab}Fab15CIM{Key-Enter}"
...
```
So `TsmcWebApp.Login(string blah, ..)` becomes `TsmcWebApp.Login()` <-- No parameters!
By doing so, only framework code and `io-res.txt` needs to be modified. To apply the patch, just copy and replace `FrameWork.dll` and `io-res.txt` in the directory of each production Korat APs.
However, it fails if the parameters to be feeded to `TsmcWebApp.Login()` are dynamically generated at runtime. So the `io-res.txt` solution fails.

## Solutions
Here lists all possible solutions for behavior change.

### Solution 1: The BehaviorsPicker (Factory)
A `BehaviorsPicker` reads the config from config file, or receive invocation with generic type `T` specified, then, returns corresponding `Behaviors` back to invoker.

#### Goal
Replace behaviors to different version with only one line of change of code.

#### Basic Usage
```csharp
// Through config file.
IBrowserBehaviors browser = BrowserBehaviorsPicker.Pick("path-to-config-file");

// Through generic type.
Chrome60 browser = BrowserBehaviorsPicker.Pick<Chrome60>();
```

#### Framework Implementation Detail
```csharp
public static class BrowserBehaviorsPicker
{
    public static BrowserBehaviors Pick(string configFilePath)
    {
        Config c = Config.Load(configFilePath);
        if (c.Distribution == "Chrome")
        {
            return PickChrome(c.Version);
        }
        if (c.Distribution == "IE")
        {
            return PickIe(c.Version);
        }
        
        throw new ArgumentException("Browser " + c.Distribution + " is not supported");
    }
    
    private static BrowserBehaviors PickChrome(string version)
    {
        switch (version)
        {
            case "55": return new Chrome55();
            case "60": return new Chrome60();
            default:
                throw new ArgumentException("Invalid Chrome version");
        }
    }
    
    private static BrowserBehaviors PickIe(string version)
    {
        // Picks IE.
    }
}
```

### Solution 2: Inteface Restriction + Ancestor Composition
Hey! The `Behaviors` class does not have any method. It's created just for the generic constrain in `BrowserPicker.Pick<T>()`.
![Imgur](http://i.imgur.com/cFlvkWX.png)

#### Basic usage
In the `Main()` of Korat AP
```csharp
IBrowserBehaviors browser = BrowserPicker.Pick<Chrome60>();
IBrowserBehaviors browser = BrowserPicker.Pick<Ie7>();
IBrowserBehaviors browser = BrowserPicker.Pick<Ie8>();

browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
```

#### Replacement
To perform specificity task
```csharp
Chrome60 browser = BrowserPicker.Pick<Chrome60>();
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
browser.OpenConsole();

browser.ChromeOnly(); //<-- Performs specificity task.
```
Someday if we need to change it to IE, then it should means that `ChromeOnly();` is no longer needed.
```csharp
// Chanage to IE7.
Ie7 browser = BrowserPicker.Pick<Ie7>();
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw"); //<-- Not influenced
browser.OpenConsole(); //<-- Not influenced

browser.ChromeOnly(); //<-- Only this line spawns the compile error, delete it.
```

#### Framework Implementation Detail
```csharp
public class Chrome60 : IBrowserBehaviors
{
    protected IBrowserBehaviors Ancestor;
    
    public Chrome60()
    {
        Ancestor = new Chrome55();
    }
    
    public void LaunchUrl(string url);
    {
        Ancestor.LaunchUrl(url);
    }
    
    public void OpenJsConsole()
    {
        Korat.SendCompositeKeys(new HashSet<Keys> { Keys.Control, Keys.Shift, Keys.J });
    }
}
```

### Solution 3: Deep Inheritance Chain
![Imgur](http://i.imgur.com/zR4NVb9.png)

#### Basic usage
In the `Main()` of Korat AP
```csharp
BrowserBehaviors browser = BrowserPicker.Pick<Chrome60>();
BrowserBehaviors browser = BrowserPicker.Pick<Ie7>();
BrowserBehaviors browser = BrowserPicker.Pick<Ie8>();

browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
```

#### Replacement
To perform browser specificity task
```csharp
Chrome60 browser = BrowserPicker.Pick<Chrome60>();
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
browser.OpenConsole();

browser.ChromeOnly(); //<-- Perform specificity task.
```
Someday if we need to change it to IE, then it should means that `ChromeOnly();` is no longer needed.
```csharp
// Chanage to IE7.
Ie7 browser = BrowserPicker.Pick<Ie7>();
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw"); //<-- Not influenced
browser.OpenConsole(); //<-- Not influenced

browser.ChromeOnly(); //<-- Only this line spawns the compile error, delete it.
```

#### Framework Implementation Detail
```csharp
public class Chrome60 : Chrome55
{
    public Chrome60() : base(Chrome55)
    {
        // Initialization.
    }
    
    public override void Login(string username, string password)
    {
        // Login behavior is different.
    }
    
    // Other behaviors unchanged...
}
```

### Solution 4: Delegate Assignment (Strategy)

![Imgur](http://i.imgur.com/CKqquEt.png)

#### Basic usage
In the `Main()` of Korat AP
```csharp
BrowserBehaviors browser = BrowserPicker.Pick("Chrome60");
BrowserBehaviors browser = BrowserPicker.Pick("Ie7");
BrowserBehaviors browser = BrowserPicker.Pick("Ie8");

browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
```

#### Replacement
To perform browser specificity task
```csharp
BrowserBehaviors browser = BrowserPicker.Pick("Chrome60");
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw");
browser.OpenConsole();

browser.ChromeOnly(); //<-- Perform specificity task.
```
Someday if we need to change it to IE, then it should means that `ChromeOnly();` is no longer needed.
```csharp
// Chanage to IE7.
BrowserBehaviors browser = BrowserPicker.Pick("IE7");
browser.LaunchUrl("http://jimmy5566.tsmc.com.tw"); //<-- Not influenced
browser.OpenConsole(); //<-- Not influenced

browser.ChromeOnly(); //<-- null reference exception at runtime
```

#### Framework Implementation Detail
BrowserBehaviors:
```csharp
public class BrowserBehaviors
{
    public delegate void PrevPage();
    public delegate void NextPage();
    public delegate void OpenJsConsole();
    public delegate void LaunchUrl(string url);

    public BrowserBehaviros()
    {
        // Initialization.
    }
    
    PrevPage PrevPage { public get; internal set; }
    PrevPage NextPage { public get; internal set; }
    OpenJsConsole OpenJsConsole { public get; internal set; }
    LaunchUrl LaunchUrl { public get; internal set; }
    // A lot of behaviors. Even IE, Chrome specificity behaviors are declared here as well.
}
```

Strategies:
```csharp
public static class Chrome55
{
    public static void PrevPage()
    {
        // Go to previous page.
    }
    
    public static void NextPage()
    {
        // Go to next page.
    }
    
    public static void OpenJsConsole()
    {
        // Opens the developers tool.
    }
    
    public static void LaunchUrl(string url)
    {
        // Go to the URL.
    }
}

public static class Chrome60
{
    public static void PrevPage()
    {
        Chrome55.PrevPage();
    }
    
    public static void NextPage()
    {
        Chrome55.NextPage();
    }
    
    public static void OpenJsConsole()
    {
        // Behavior changed, concrete implementation here.
    }
    
    public static void LaunchUrl(string url)
    {
        Chrome55.LaunchUrl(url);
    }
}
```

Strategy picker:
```csharp
public static class BrowserPicker
{
    public static BrowserBehaviors Pick(string version)
    {
        BrowserBehaviors b = new BrowserBehaviors();
    
        if (version.Contains("Chrome") && version.Contains("55"))
        {
            b.NextPage += Chrome55.NextPage;
            b.PrevPage += Chrome55.PrevPage;
            b.OpenJsConsole += Chrome55.OpenJsConsole;
            b.LaunchUrl += Chrome55.LaunchUrl;
        }
        if  (version.Contains("Chrome") && version.Contains("60"))
        {
            b.OpenJsConsole -= Chrome55.OpenJsConsole;
            b.OpenJsConsole += Chrome60.OpenJsConsole;
        }
        // A lot of if...
        
        return b;
    }
}
```

### Solution 0: BehaviorsPool
The `BehaviorsPool` allows each behavior instance queries the other behaviors for help.

#### Goal
Each behavior may requires the help from the other behaviors. For example, if the `BrowserBehaviors` has a method named `GetCurrentUrl()` which returns the current URL of web page, it requries the help of `OsBehaviors.Copy()`. Thus, the `BehaviorsPool` is created for requesting all instantiated behaviors.

#### Basic Usage
```csharp
public class BrowserBehaviors
{
    protected OsBehaviors Os;

    public BrowserBehaviors(BehaviorsPool pool)
    {
        Os = pool.Request<OsBehaviors>();
        // Initialization...
    }
    
    public string GetCurrentUrl()
    {
        this.FocusUrlBar();
        return Os.Copy();
    }
}
```

#### Framework Implementation Detail
```csharp
public class BehaviorsPool
{
    private List<object> _behaviors;

    public BehaviorsPool()
    {
        _behaviors = new List<Behaviors>();
    }
    
    public bool HasBehaviorsType<T>()
    {
        foreach (object behaviors in _behaviors)
        {
            if (behaviors is T)
            {
                return true;
            }
        }

        return false;
    }

    public void Add<T>(T behaviors)
    {
        if (behaviors == null)
        {
            throw new ArgumentNullException("Given behaviors should not be null.");
        }
        
        if (!HasBehaviorsType<T>())
        {
            _behaviorsPool.Add(behaviors);
        }

        throw new ArgumentException("Given behavior already existed in behaviors pool.");
    }
    
    public T Request<T>()
    {
        foreach (Behaviors behaviors in _behaviors)
        {
            T actualBehaviors = behaviors as T;

            if (actualBehaviors != null)
            {
                return actualBehaviors;
            }
        }

        return null;
    }
}
```

# Design: Image Change
Images now are bound to behaviors. Provides `ImageLoader` that automatically loads image files directly.
~~Provides a `ImagePicker` class and a configuration file. Loads the appropriate images from specific location / server to the resource directory.~~

## Framework Implementation Detail (Updated)
### Behaviors.cs
```csharp
public class Behaviors<TImage>
{
    public T Images { public get; protected set; }

    public Behaviors(StorageInfo info)
    {
        Images = ImageLoader.Load<T>(info);
    }
}
```

### ImageLoader.cs
```csharp
public static class ImageLoader
{
    public static T Load<T>(StorageInfo info)
    {
        Storage storage = StorageFactory.Make(info);
        T images = Activator.CreateInstance<T>();
        
        foreach (PropertyInfo property in typeof(T).GetProperties())
        {
            typeof(T).InvokeMember(
                property.Name,
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.SetProperty | BindingFlags.SetField,
                Type.DefaultBinder,
                images,
                new object[] { storage.Load(property.Name) }
            );
        }
        
        return images;
    }
}
```

### StorageFactory.cs
```csharp
public static class StorageFactory
{
    public static Storage Make(StorageInfo storageInfo)
    {
        List<StorageInfo> infos = LoadConfig("storage-config-path");
        StorageInfo info = Find(infos, storageInfo.StorageName);
        
        switch (info.StorageType)
        {
            case "ftp":
                return new FtpStorage(info.Username, info.Password, info.Path, info.Port, storageInfo.SubPath);
            case "http-server":
                return new HttpStorage(info.Username, info.Password info.Path, info.Port, storageInfo.SubPath);
            case "local":
                return new LocalStorage(null, null, info.Path, null, storageInfo.SubPath);
            default:
                throw new ArgumentException("No such storage type.");
        }
    }
}
```

## ~~Basic Usage~~
```csharp
BrowserImages images = ImagePicker.Pick<BrowserImages>("path-to-config-file.conf");
```

## ~~Framework Implementation Detail~~
```csharp
public static class ImagePicker
{
    public static T Pick<T>(string configPath)
    {
        string[] configEntries = File.ReadAllLines(configPath);
        object images = ReflectionGetImages(T);
        
        foreach (string entry in configEntries)
        {
            string tokens[] = entry.Split(' ');
            string imageName = tokens[0];
            string resourceLocation = tokens[1];
            
            string loadedResourcePath = Loader.Load(resourceLocation, Config.ResourcePath);
            // Throws exception once reflection failed. For example, no such property "imageName" in images.
            ReflectionAssignValue(images, imageName, loadedResourcePath);
        }
        
        return images as T;
    }
}
```

# Design: Logging
Here lists all requirements and solutions for logging.

## Usage
How user uses logger?
- User can leave it to Korat Framework passively
- User can perform logging him / herself proactively

1. Passively: Never catch the exception, let it bubbles up to Korat Framework. Korat Framework would handle it.
2. Proactively: `try` / `catch` the interested block, use `Framework.Logger.Info()` / `Framework.Logger.Warning()` / `Framework.Logger.Debug()` / `Framework.Logger.Error()` and so on to log proactively.
3. User can implements `Framework.ILogger` to create customized logger, and attatches it to `Framework` by `Framework.Logger.Attatch(new MyLogger())`.
4. User can disable certain exceptions.
5. User can enable / disable logging by `Framework.Logger.Enable();` / `Framework.Logger.Disable();`

## The Handler
The `Handler` class is a class that processes exceptions before reported.

### Basic Usage
```csharp
public class Handler : IHandler
{
    public override Type[] IgnoredExceptions { get; } = 
    {
        WaitFailedException, RecognitionFailedException  
    };
    
    public void Handle(Exception exception)
    {
        // process exceptions here.
    }
    
    public void Report(Exception exception, Logger logger)
    {
        logger.Error(exception);
    }
}
```

### Framework Implementation Detail
```csharp
public interface IHandler
{
    Type[] IgnoredExceptions { get; };
    
    void Handle(Exception exception);
    
    void Report(Exception exception, Logger logger);
}
```

## Design
Here is the system design of logger module.
![Imgur](http://i.imgur.com/gpb81jA.png)

# Design: How to Force User Use Korat Framework in the Right Way
Why to force the way user use Korat Framework?
1. Ensures "one runninig Korat instance one time"
2. Ensures the developer follows the framework design, thus ensures the ease of replacement of "user generated behavior"

Solution: Provides Visual Studio template file, so there will be a `Korat Project Template` item while user trying to create a new Visual Studio project. For example: After user selects this template, following file are automatically generated and applied:

## Behavior Template
`IApplicationBehaviors.cs`
```csharp
// Please rename it to a name that best matches your target application.
interface IApplicationBehaviors
{
    
}
```

`ApplicationBehaviors.cs`
```csharp
// Please rename it to a name that best matches your target application.
class ApplicationBehaviors : Behaviors, IApplicationBehaviors
{
    protected ApplicationImages Images;

    public ApplicationBehaviors(Korat korat, BehaviorsPool pool) : base(korat, pool)
    {
        public override string Version { get; } = "1.0";
    }
}
```

`ApplicationImages.cs`
```csharp
// Please rename it to a name that best matches your target application.
class ApplicationImages
{
    // Lists all images your "ApplicationImages" needs here.
}
```

## Project Template
Reminds: Behavior Template is also included while creating a Korat project with `Korat Framework Project Template`.

`Main.cs`
```csharp
static class Program
{
    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    static void Main()
    {
        // Or move the mutex stuff into KoratFramework.Bootstrap();
        Mutex locker = new Mutex(true, "KORAT");
        KoratFramework framework = new KoratFramework();
        
        if(locker.WaitOne(TimeSpan.Zero, true))
        {
            framework.Bootstrap(
                Resources.ConfigFilePath,
                new BehaviorsProvider(framework.Korat, framework.BehaviorsPool),
                new Handler()
            );
            framework.Run(new Script(framework.Korat, framework.BehaviorsPool));
            mutex.ReleaseMutex();
        }
        else
        {
            framework.Logger.LogSingleRunningInstanceError();
        }
    }
}
```

~~`BehaviorsProvider.cs`~~ => Each behavior is defined in the configuration file. Use reflection to load them.
```csharp
class BehaviorsProvider
{
    public BehaviorsProvider(Korat korat, BehaviorsPool pool)
    {
        pool.Add(new ApplicationBehaviors(korat, pool));
    }
}
```

`Script.cs`
```csharp
class Script : Behaviors, IRunnable
{
    public Script(Korat korat, BehaviorsPool pool) : base(korat, pool)
    {
        
    }

    public override void Run()
    {
        // As a professional director, sway your creativity here!
    }
}
```

`Handler.cs`
```csharp
public class Handler : IHandler
{
    public override Type[] IgnoredExceptions { get; } = 
    {
        WaitFailedException, RecognitionFailedException  
    };
    
    public void Handle(Exception exception)
    {
        // process exceptions here.
    }
    
    public void Report(Exception exception, Logger logger)
    {
        logger.Error(exception);
    }
}
```

# Design: Configuration File
Here designs
- How configuration files are structured
- Which configuration files should be created
- Available entries in each configuration file

Config file solutions:
1. Native C# .NET `.conf` file, (remember to add these `.conf` file into `.gitignore`)
2. [Newtonsoft.json](http://www.newtonsoft.com/json)

C# .NET configuration tutorials:
- https://stackoverflow.com/questions/1316058/how-to-create-custom-config-section-in-app-config
- https://msdn.microsoft.com/en-us/library/2tw134k3.aspx

## App
The application config file.

### Config File
```javascript
{
    "Name": "AecQcMonBt",
    "Env": "production",
    "Author": "Tzu-Chao, Wang",
    "Manager": "Jimmy5566",
    "Contact": "john29917958@gmail.com"
}
```

## Log
Log configuration file should be able to handle both local and remote logs.

### Config file
```javascript
[
    {
        "Name": "local",
        "Location": "C:\Users\Jimmy5566\Documents\KLog.txt",
        "Level": "all"
    },
    {
        "Name": "local",
        "Location": ".\KLog.txt",
        "Level": "debug"
    },
    {
        "Name": "remote",
        "Location": "10.197.jimmy.5566",
        "Level": "error",
        "username": "jimmy",
        "password": "babysitter",
        "database": "5566",
        "table": "pychen.zi",
        "type": "oracle"
    }
]
```

### How User Can Extend Custome Logger
Demonstrates how user can extends the framework logger.

#### MyLogger.cs
```csharp
class MyLogger : Logger
{
    public MyLogger(string location, Level level) : base(location, level)
    {
    
    }
}
```

#### Registers MyLogger.cs to Framework
```csharp
Framework.Loggers.Register("my-logger", (jToken) =>
{
    // Here initializes my own new logger.
    MyLogger logger = new MyLogger(jToken["location"], jToken["level"], jToken["my-custome-config-entry"]);
    return logger;
});
```

#### Add "my-logger" to Config File
```javascript
[
    {
        "Name": "my-logger",
        "Location": "./MyLoggerRecord/my-logger.txt",
        "Level": "all",
        "my-custome-config-entry": "5566"
    },
    {
        "Name": "local",
        "Location": "C:\Users\Jimmy5566\Documents\KLog.txt",
        "Level": "all"
    },
    {
        "Name": "local",
        "Location": ".\KLog.txt",
        "Level": "debug"
    },
    {
        "Name": "remote",
        "Location": "10.197.jimmy.5566",
        "Level": "error",
        "username": "jimmy",
        "password": "babysitter",
        "database": "5566",
        "table": "pychen.zi",
        "type": "oracle"
    }
]
```

### Framework Mapping File
```csharp
// User custome logger can be appened to this dictionary.
Dictionary<string, Func<JToken, Logger>> LoggerInitFuncs = new Dictionary<string, Func<JToken, Logger>>
{
    { "local", (json) => new TextLogger(json["location"].Value<string>(), json["level"].Value<string>()) },
    { "remote", (json) => new OracleLogger(json["location"].Value<string>(), json["level"].Value<string>(), json["username"].Value<string>(), json["password"].Value<string>(), json["db"].Value<string>()) }
};

JObject configData = JObject.Parse(Load());
foreach (KeyValuePair<string, JToken> logger in configData)
{
    string loggerType = logger.Value["Name"].Value<string>();
    
    if (!loggersMap.HasKey(loggerType))
    {
        throw new ArgumentException("No such logger");
    }
    
    LoggersPool.Add(LoggerInitFuncs[loggerType]);
}
```

## Behaviors and Images
User should be able to add / replace all required behaviors in config file.

### Config file
Includes `behaviors.conf` and `image-sources.conf`.

#### Behaviors Config File
```javascript
[
    {
        "Name": "Chrome",
        "Version": "60",
        "Images":
        {
            "Storage": ["korat-image-storage-1"],
            "UrlBar": "url-bar.png",
            "Tab": "browser-tab.png",
            "...": "..."
        }
    },
    {
        "Name": "Windows Server",
        "Version": "2012",
        "Images":
        {
            "Storage": ["korat-image-storage-1", "korat-image-storage-2"],
            "Win": "win.png",
            "SearchBar": "quick-serch-bar.png",
            "...": "..."
        }
    },
    {
        "Name": "ECP",
        "Version": "1.0",
        "Images":
        {
            "Storage": ["local"],
            "AppCollectionMenu": "app-collection-menu.png",
            "DropdownButton": "dropdown.png",
            "...": "..."
        }
    }
]
```

#### Image Sources Config File
```javascript
[
    {
        "Name": "korat-image-storage-1",
        "StorageType": "ftp",
        "Port": 21,
        "Username": "Jimmy",
        "Password": 5566,
        "Path": "10.150.188.xx/korat/win-server-2012",
    },
    {
        "Name": "korat-image-storage-2",
        "StorageType": "http-server",
        "Port": 80,
        "Username": "Jimmy",
        "Password": 5566,
        "Path": "10.150.188.xx/korat/http-img-pool/",
    },
    {
        "Name": "local",
        "StorageType": "local",
        "Port": null,
        "Username": null,
        "Password": null,
        "Path": "./local-img-storage/"
    }
    {
        "...": "..."
    }
]
```

## Config File Structure
Groups following configuration files into `app-root\configs\` diretory:
- app.conf
- loggers.conf
- log.conf
- behaviors.conf
- resources.conf

### How User Can Extend Custome Behaviors
#### Creates User's Own New Behaviors
`IApplicationBehaviors.cs`
```csharp
// Please rename it to a name that best matches your target application.
interface IApplicationBehaviors
{
    
}
```

`ApplicationBehaviors.cs`
```csharp
// Please rename it to a name that best matches your target application.
class ApplicationBehaviors : Behaviors, IApplicationBehaviors
{
    protected ApplicationImages Images;

    public ApplicationBehaviors(Korat korat, BehaviorsPool pool) : base(korat, pool)
    {
        public override string Version { get; } = "1.0";
    }
}
```

#### Registers Behaviors to Framework
```csharp
Framework.Behaviors.Register("app-name", "1.0", typeof(ApplicationBehaviors));
```

#### Adds "app-name" to Config File
```javascript
[
    {
        "Name": "Windows Server",
        "Version": "2012"
    },
    {
        "Name": "Chrome",
        "Version": "60"
    },
    {
        "Name": "ECP",
        "Version": "1.0"
    },
    {
        "Name": "app-name",
        "Version": "1.0"
    }
]
```

### Framework Mapping File
```csharp
// User custome behaviors can be appended to this dictionary.
Dictionary<BehaviorsInfo, Type> map = new Dictionary<BehaviorsInfo, Type>()
{
    { new BehaviorsInfo("Chrome", "60"), typeof(Chrome60) },
    { new BehaviorsInfo("Chrome", "55"), typeof(Chrome55) },
    { new BehaviorsInfo("Windows Server", "2012"), typeof(WinServer2012) }
};

class Comparer : IEqualityComparer<BehaviorsInfo>
{
    public bool Equals(BehaviorsInfo x, BehaviorsInfo y)
    {
        return x.App == y.App && x.Version == y.Version;
    }

    public int GetHashCode(BehaviorsInfo obj)
    {
        return obj.GetHashCode();
    }
}

List<BehaviorsInfo> infos = Config.LoadInfos("path-to-config-file");
foreach (BehaviorsInfo info in infos)
{
    if (!map.Keys.Contains(info, new Comparer()))
    {
        throw new ArgumentException("No such behaviors.");
    }
    
    Type t = map[info];
    object behaviors = Activator.CreateInstance(t, Korat, BehaviorsPool);
    BehaviorsPool.Add(behaviors);
}
```

# Design: Framework Directory (Namespace) Structure
Here sets the directory structure for each framework class files. The directory path should be the namespace of class file.

```
App/
    App.cs
    Korat/
        Korat.cs
        Keyboard.cs
        Mouse.cs
        Recognizer.cs
Behaviors/
    BehaviorsPool.cs
    BehaviorsServiceProvider.cs
    Behaviors/
        Behaviors.cs
        Os/
            IOsBehaviors.cs
            Windows/
                WindowsImages.cs
                Windows8Images.cs
                Windows.cs
                Win7.cs
                Win8.cs
            Ubuntu/
                UbuntuImages.cs
                Ubuntu.cs
                Ubuntu14-04.cs
                Ubuntu16.cs
        Browser/
            IBrowserBehaviors.cs
            Chrome.cs
            Ie.cs
            Firefox.cs
Components/
    Component.cs
    ComponentSearcher.cs
    Dropdown.cs
    Scrollable.cs
Config/
    Config.cs
    RecognitionConfig.cs
    App/
        AppConfig.cs
        AppConfigLoader.cs
        app.conf
    Log/
        LogConfig.cs
        LogConfigLoader.cs
        loggers.conf
        log.conf
    Behaviors/
        BehaviorsConfig.cs
        BehaviorsConfigLoader.cs
        BehaviorsConfigEntry.cs
        BehaviorsConfigEntryComparer.cs
        behaviors.conf
Exceptions/
    ExceptionA.cs
    ExceptionB.cs
ImageRecognition/
    IRecognition.cs
    SolidColorMatcher.cs
Log/
    LogServiceProvider.cs
    LogManager.cs
    Loggers/
        Logger.cs
        LocalFileLogger.cs
        FtpLogger.cs
Support/
    FileHelper.cs
    Geometry.cs
    PathHelper.cs
    ServiceProvider.cs
```

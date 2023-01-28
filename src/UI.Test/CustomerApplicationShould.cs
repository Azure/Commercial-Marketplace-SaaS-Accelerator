using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System.Reflection;

namespace Marketplace.SaaS.Accelerator.UI.Test;

/// <summary>
/// This class is used to test Customer Portal Application functionality
/// </summary>
[TestClass]
public class CustomerApplicationShould
{
    private TestContext testContextInstance;
    private IWebDriver driver;
    private string customerAppURL = string.Empty;
    private string loginUserName = string.Empty;
    private string password = string.Empty;
    private string landingPageToken = string.Empty;

    private const string customerHomePageTitle = "Welcome";
    private const string azAdSignInPageTitle = "Pick an account";

    /// <summary>
    /// The configuration.
    /// </summary>
    private TestConfiguration configuration = new TestConfiguration();


    public CustomerApplicationShould()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
                                    .AddJsonFile("appsettings.test.json")
                                    .AddEnvironmentVariables()
                                    .Build();
        this.configuration = config.GetSection("AppSetting").Get<TestConfiguration>();
    }

    /// <summary>
    ///Gets or sets the test context which provides
    ///information about and functionality for the current test run.
    ///</summary>
    public TestContext TestContext
    {
        get
        {
            return testContextInstance;
        }
        set
        {
            testContextInstance = value;
        }
    }

    [TestInitialize()]
    public void Setup()
    {
        customerAppURL = configuration.customerAppURL;
        loginUserName = configuration.loginUserName;
        password = configuration.password;
        landingPageToken = configuration.landingPageToken;

        var options = new ChromeOptions();
        options.AddArguments("--incognito");
        options.AddArguments("--headless");
        options.AddArguments("--window-size=1920,1080");
        options.AddArguments("--start-maximized"); 
        driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TestCleanup()]
    public void Cleanup()
    {
        driver.Quit();
    }

    [TestMethod]
    [TestCategory("CustomerApp")]
    public void CustomerLogin()
    {
        //Arrange/Act
        driver.Navigate().GoToUrl(customerAppURL);
        //driver.FindElement(By.XPath("//a[@href ='/Account/SignIn']")).Click();
        var signInLink = new WebDriverWait(driver, TimeSpan.FromSeconds(120)).Until(driver => driver.FindElement(By.XPath("//a[@href ='/Account/SignIn']")));
        signInLink.Click();
        doLogIn();
        
        String homePageTitle = driver.FindElement(By.XPath("//*[@id='divIndex']/div/div/div[1]/h1[1]")).GetAttribute("innerHTML");
        //Assert
        Assert.AreEqual(homePageTitle, customerHomePageTitle, "Customer App LogIn Verified");
        
    }

    [TestMethod]
    [TestCategory("CustomerApp")]
    public void CustomerLogout()
    {
        //Arrange/Act
        driver.Navigate().GoToUrl(customerAppURL);
        driver.FindElement(By.XPath("//a[@href ='/Account/SignIn']")).Click();
        doLogIn();
        doLogOut();

        Thread.Sleep(1000);
        String signInPageTitle = driver.FindElement(By.XPath("//div[@id='loginHeader']/div")).GetAttribute("outerText");
        //Assert
        Assert.AreEqual(signInPageTitle, azAdSignInPageTitle, "Customer App Logout Verified");

    }


    [TestMethod]
    [TestCategory("CustomerApp")]
    public void CheckLandingPage()
    {
        //Arrange/Act
        driver.Navigate().GoToUrl($"{customerAppURL}/?token={landingPageToken}");
        doLogIn();

        //Assert
        var SubDetailsPageTile = driver.FindElement(By.ClassName("cm-section-heading")).GetAttribute("innerHTML");
        Assert.AreEqual(SubDetailsPageTile, "Subscription Details", "Customer App Landingpage Page Verified Verified");

    }


    /// <summary>
    /// Private Methods
    /// </summary>

    void doLogIn()
    {
        driver.FindElement(By.Id("i0116")).Clear();
        driver.FindElement(By.Id("i0116")).SendKeys(loginUserName);
        driver.FindElement(By.Id("i0116")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("i0118")).SendKeys(password);
        driver.FindElement(By.Id("i0118")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("idSIButton9")).SendKeys(Keys.Enter);
        //Need to add accept consent
    }

    void doLogOut()
    {
        driver.FindElement(By.XPath("//a[@href ='/Account/SignOut']")).Click();
    }
}

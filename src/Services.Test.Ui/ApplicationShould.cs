using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.Configuration;

namespace Services.Test.Ui;

/// <summary>
/// Summary description for MySeleniumTests
/// </summary>
[TestClass]
public class ApplicationShould
{
    private TestContext testContextInstance;
    private IWebDriver driver;
    private string adminAppURL;
    private string customerAppURL;
    private string loginUserName;
    private string password;

    private const string adminHomePageTitle = "Home";
    private const string customerHomePageTitle = "Welcome";
    private const string azAdSignInPageTitle = "Pick an account";

    /// <summary>
    /// The configuration.
    /// </summary>
    private TestConfiguration configuration = new TestConfiguration();


    public ApplicationShould()
    {
        IConfigurationRoot config = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
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
        adminAppURL = configuration.adminAppURL;
        customerAppURL = configuration.customerAppURL;
        loginUserName = configuration.loginUserName;
        password = configuration.password;

        var options = new ChromeOptions();
        options.AddArguments("--incognito");
        driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
    }

    [TestCleanup()]
    public void Cleanup()
    {
        driver.Quit();
    }


    [TestMethod]
    [TestCategory("AdminApp")]
    public void AdminLoginLogout()
    {
        //Arrange
        driver.Navigate().GoToUrl(adminAppURL);


        //Act
        driver.FindElement(By.Id("i0116")).Clear();
        driver.FindElement(By.Id("i0116")).SendKeys(loginUserName);
        driver.FindElement(By.Id("i0116")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("i0118")).SendKeys(password);
        driver.FindElement(By.Id("i0118")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("idSIButton9")).SendKeys(Keys.Enter);
        //Need to add accept consent
        String homePageTitle = driver.FindElement(By.XPath("//div['page-title']/h1[1]")).GetAttribute("innerHTML");
        //Assert
        Assert.AreEqual(homePageTitle, adminHomePageTitle, "Admin App Verified login");

        //Act
        driver.FindElement(By.XPath("//a[@href ='/Account/SignOut']")).Click();
        Thread.Sleep(2000);
        String signInPageTitle = driver.FindElement(By.XPath("//div[@id='loginHeader']/div")).GetAttribute("outerText");
        //Assert
        Assert.AreEqual(signInPageTitle, azAdSignInPageTitle, "Admin App Verified logout");

    }


    [TestMethod]
    [TestCategory("CustomerApp")]
    public void CustomerLoginLogout()
    {
        //Arrange
        driver.Navigate().GoToUrl(customerAppURL);

        //Act
        driver.FindElement(By.XPath("//a[@href ='/Account/SignIn']")).Click();
        Thread.Sleep(2000);
        driver.FindElement(By.Id("i0116")).Clear();
        driver.FindElement(By.Id("i0116")).SendKeys(loginUserName);
        driver.FindElement(By.Id("i0116")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("i0118")).SendKeys(password);
        driver.FindElement(By.Id("i0118")).SendKeys(Keys.Enter);
        Thread.Sleep(2000);
        driver.FindElement(By.Id("idSIButton9")).SendKeys(Keys.Enter);
        //Need to add accept consent
        String homePageTitle = driver.FindElement(By.XPath("//*[@id='divIndex']/div/div/div[1]/h1[1]")).GetAttribute("innerHTML");
        //Assert
        Assert.AreEqual(homePageTitle, customerHomePageTitle, "Customer App Verified login");

        //Act
        driver.FindElement(By.XPath("//a[@href ='/Account/SignOut']")).Click();
        Thread.Sleep(2000);
        String signInPageTitle = driver.FindElement(By.XPath("//div[@id='loginHeader']/div")).GetAttribute("outerText");
        //Assert
        Assert.AreEqual(signInPageTitle, azAdSignInPageTitle, "Admin App Verified logout");

    }

}
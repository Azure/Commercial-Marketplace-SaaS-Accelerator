using Microsoft.Extensions.Configuration;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using System.Reflection;
using OpenQA.Selenium.Support.UI;

namespace Marketplace.SaaS.Accelerator.UI.Test;

/// <summary>
/// This class is used to test Admin Application functionality
/// </summary>
[TestClass]
public class AdminApplicationShould
{
    private TestContext testContextInstance;
    private IWebDriver driver;
    private string adminAppURL = string.Empty;
    private string loginUserName = string.Empty;
    private string password = string.Empty;


    /// <summary>
    /// The configuration.
    /// </summary>
    private TestConfiguration configuration = new TestConfiguration();


    public AdminApplicationShould()
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
        adminAppURL = configuration.adminAppURL;
        loginUserName = configuration.loginUserName;
        password = configuration.password;

        var options = new ChromeOptions();
        options.AddArguments("--incognito");
        options.AddArguments("--headless");
        options.AddArguments("--window-size=1920,1080");
        options.AddArguments("--start-maximized");
        driver = new ChromeDriver(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), options);
        driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(60);
    }

    [TestCleanup()]
    public void Cleanup()
    {
        driver.Quit();
    }

    [TestMethod]
    [TestCategory("AdminApp")]
    public void AdminAppLogin()
    {
        //Arrange/Act
        doLogIn();
        
        
        //Assert
        String homePageTitle = driver.FindElement(By.XPath("//div['page-title']/h1[1]")).GetAttribute("innerHTML");
        Assert.AreEqual(homePageTitle, Constants.AdminHomePageTitle, "Admin App Verified login");
    }

    [TestMethod]
    [TestCategory("AdminApp")]
    public void AdminAppLogout()
    {
        //Arrange/Act
        doLogIn();
        doLogOut();
        Thread.Sleep(1000);
        
        //Assert
        String signInPageTitle = driver.FindElement(By.XPath("//div[@id='loginHeader']/div")).GetAttribute("outerText");
        Assert.AreEqual(signInPageTitle, Constants.AzAdSignInPageTitle, "Admin App Verified logout");

    }

    [TestMethod]
    [TestCategory("AdminApp")]
    public void AdminAppGotoSubscriptionsPage()
    {
        //Arrange
        doLogIn();

        //Act
        gotoSubscriptionsPageFromTile();
        
        //Assert
        var SubPageTile = driver.FindElement(By.ClassName("cm-section-heading")).GetAttribute("innerHTML");
        Assert.AreEqual(SubPageTile, Constants.AdminSubscriptionPageTitle, "Admin App Subscription Page Navigation Verified");
    }

    [TestMethod]
    [TestCategory("AdminApp")]
    public void AdminAppSubscriptionsPageFetchAll()
    {
        //Arrange
        doLogIn();
        gotoSubscriptionsPageFromTile();

        //Act
        driver.FindElement(By.XPath("//a[@class='btn btn-secondary cm-button']")).Click();

        //Click OK for Javascript confirm on Fetch Click
        IAlert alert = driver.SwitchTo().Alert();
        string text = alert.Text;
        alert.Accept();

        //Check In Progress
        var SwalInProgress = driver.FindElement(By.ClassName("swal-title")).GetAttribute("innerHTML");
        Assert.AreEqual(SwalInProgress, Constants.SwalInprogress, "Fetch InProgress working");

        //Check Complete
        var swalOkButton = new WebDriverWait(driver, TimeSpan.FromSeconds(120)).Until(driver => driver.FindElement(By.ClassName("swal-button-container")));
        var SwalComplete = driver.FindElement(By.ClassName("swal-title")).GetAttribute("innerHTML");
        Assert.AreEqual(SwalComplete, Constants.SwalComplete, "Fetch Complete working");

        //Refresh Subscriptions Page
        swalOkButton.Click();
        driver.Navigate().Refresh();

        //Check if rows populated
        var resultRows = driver.FindElements(By.XPath("//table/tbody/tr"));
        Assert.IsNotNull(resultRows);
    }


    /// <summary>
    /// Private Methods
    /// </summary>

    void doLogIn()
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
    }

    void doLogOut()
    {
        //driver.FindElement(By.XPath("//a[@href ='/Account/SignOut']")).Click();
        var logoutbtn = new WebDriverWait(driver, TimeSpan.FromSeconds(120)).Until(driver => driver.FindElement(By.XPath("//a[@href ='/Account/SignOut']")));
        logoutbtn.Click();
    }

    void gotoSubscriptionsPageFromTile()
    {
        driver.FindElement(By.XPath("//*[@id='onedash-menu-homepage1']/a")).Click();
    }

    void gotoSubscriptionsPageFromMenu()
    {
        //driver.FindElement(By.XPath("//a[@href ='/Home/Subscriptions']")).Click();
    }

    void gotoPlansPage()
    {
        //driver.FindElement(By.XPath("//a[@href ='/Plans']")).Click();

    }

    void gotoOffersPage()
    {
        //driver.FindElement(By.XPath("//a[@href ='/Offers']")).Click();

    }

    void gotoUsersPage()
    {
        //driver.FindElement(By.XPath("//a[@href ='/KnownUsers']")).Click();

    }

    
    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void ChangePlan()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void ChangeQuantity()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckPlansPage()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckSubscriptionsPage()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckOffersPage()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckUsersPage()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckSchedulerPage()
    {
        //Arrange

        //Act

        //Assert

    }

    //todo
    [TestMethod]
    [TestCategory("AdminApp")]
    public void CheckAppConfigPage()
    {
        //Arrange

        //Act

        //Assert

    }
}

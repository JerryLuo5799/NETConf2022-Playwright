const { chromium, devices } = require('playwright');

const device = devices['iPhone 12'];

(async () => {
  const browser = await chromium.launch({
    headless: false
  });
  const context = await browser.newContext({
    recordVideo:{ 
        dir:"video"
    },
  //  ...device
  });

  context.tracing.start({ screenshots: true, snapshots: true, sources: true});

  const page = await context.newPage();
  await page.goto('https://playwright.dev/');
  await page.getByRole('button', { name: 'Node.js' }).click();
  await page.getByRole('navigation').getByRole('link', { name: 'Python' }).click();
  await page.getByRole('button', { name: 'Python' }).click();
  await page.getByRole('navigation').getByRole('link', { name: '.NET' }).click();
  await page.getByRole('link', { name: 'Docs' }).click();
  await page.getByRole('navigation').filter({ hasText: 'Playwright for .NETDocsAPI.NET.NETNode.jsPythonJavaCommunitySearchK' }).getByRole('link', { name: 'API' }).click();
  await page.getByRole('button', { name: 'Search' }).click();
  await page.getByPlaceholder('Search docs').click();
  await page.getByPlaceholder('Search docs').fill('click');
  await page.getByRole('link', { name: 'Mouse.ClickAsync(x, y, options)​ Mouse' }).click();

  context.tracing.stop({path: "trace.zip"});

  //await page.screenshot({path:`Hello_Playwright.png`, fullPage: true});

  // ---------------------
  await context.close();
  await browser.close();
})();
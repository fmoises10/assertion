using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [OneTimeSetUp]
    public void GlobalSetup()
    {
        SetDefaultExpectTimeout(10_000);
    }

    [Test]
    public async Task ButtonTextChangesAfterInput()
    {
        // Navegar a la página
        await Page.GotoAsync("http://uitestingplayground.com/textinput");

        // Verificar que el botón es visible
        var button = Page.Locator("#updatingButton");
        await Expect(button).ToBeVisibleAsync();

        // Verificar que el botón está habilitado
        await Expect(button).ToBeEnabledAsync();

        // Encontrar el campo de texto y escribir un nombre
        var inputField = Page.Locator("#newButtonName");
        string name = "Félix";
        await inputField.FillAsync(name);

        // Hacer clic en el botón
        await button.ClickAsync();

        // Verificar que el texto del botón cambió al nombre introducido
        await Expect(button).ToHaveTextAsync(name);
    }
}
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;
using Microsoft.Playwright;
using Microsoft.Playwright.NUnit;
using NUnit.Framework;

namespace PlaywrightTests;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    private JSchema _buttonSchema;

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        SetDefaultExpectTimeout(10_000);

        // Cargar el esquema JSON desde el archivo
        string schemaJson = File.ReadAllText("buttonSchema.json");
        _buttonSchema = JSchema.Parse(schemaJson);
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

        // Validar el texto del botón contra el esquema JSON
        string buttonText = await button.TextContentAsync();
        JObject jsonData = new JObject { ["buttonText"] = buttonText };

        Assert.That(jsonData.IsValid(_buttonSchema), Is.True, "El texto del botón no cumple con el esquema JSON.");
    }
}

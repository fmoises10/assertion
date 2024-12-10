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
    private JSchema _buttonSchema; // Define el esquema JSON

    [OneTimeSetUp]
    public void GlobalSetup()
    {
        SetDefaultExpectTimeout(10_000);

        // Cargar el esquema JSON desde un archivo
        string schemaJson = File.ReadAllText("buttonSchema.json");
        _buttonSchema = JSchema.Parse(schemaJson);
    }

    [Test]
    public async Task ButtonTextWithInvalidData()
    {
        // Navegar a la página
        await Page.GotoAsync("http://uitestingplayground.com/textinput");

        // Verificar que el botón es visible
        var button = Page.Locator("#updatingButton");
        await Expect(button).ToBeVisibleAsync();

        // Verificar que el botón está habilitado
        await Expect(button).ToBeEnabledAsync();

        // Introducir un valor inválido en el campo de texto
        var inputField = Page.Locator("#newButtonName");
        string invalidName = "123!@#"; // Valor que no cumple con el esquema
        await inputField.FillAsync(invalidName);

        // Hacer clic en el botón
        await button.ClickAsync();

        // Verificar que el texto del botón cambió
        await Expect(button).ToHaveTextAsync(invalidName);

        // Validar el texto contra el esquema JSON
        string buttonText = await button.TextContentAsync();
        JObject jsonData = new JObject { ["buttonText"] = buttonText };

        // Asegurarse de que el valor no sea válido según el esquema
        bool isValid = jsonData.IsValid(_buttonSchema);
        Assert.That(isValid, Is.False, "El texto del botón no debería cumplir con el esquema JSON.");
    }
}

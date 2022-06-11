window.onload = function () {
    if (document.querySelector("#productSelector")) {
        document.querySelector("#productSelector").addEventListener("change", getProduct)
    }
}

function getProduct(evt) {
    let product = document.querySelector("#productSelector");
    let text = product.options[product.selectedIndex].text;

    let pieces = text.split("$");
    let price = pieces[1].trim();
    let area = document.querySelector("#unitPrice");

    area.value = `${price}`
}
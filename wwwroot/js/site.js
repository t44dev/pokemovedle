// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// antiforgery
document.addEventListener("htmx:configRequest", (evt) => {
    let httpVerb = evt.detail.verb.toUpperCase();
    if (httpVerb === 'GET') return;
    
    let antiForgery = htmx.config.antiForgery;

    if (antiForgery) {
        
        // already specified on form, short circuit
        if (evt.detail.parameters[antiForgery.formFieldName])
            return;
        
        if (antiForgery.headerName) {
            evt.detail.headers[antiForgery.headerName]
                = antiForgery.requestToken;
        } else {
             evt.detail.parameters[antiForgery.formFieldName]
                = antiForgery.requestToken;
        }
    }
});

// local storage handling
const persistGuess = (name) => {
    currentValue = localStorage.getItem("moves");
    if (currentValue == null) {
        result = name
    } else {
        result = currentValue + "," + name
    }
    localStorage.setItem("moves", result)
}

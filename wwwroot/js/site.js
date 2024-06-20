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
    if (currentValue == null || currentValue == "") {
        result = name
    } else {
        result = currentValue + "," + name
    }
    localStorage.setItem("moves", result)
}

// timer
const updateTimer = (finalTime) => {
    result = false;
    let ms = finalTime - Date.now();
    let hours = "00", minutes = "00", seconds = "00";
    if (ms >= 0) {
        hours = String(Math.floor(ms/3600000)).padStart(2, '0');
        ms = ms - (hours * 3600000);
        minutes = String(Math.floor(ms/60000)).padStart(2, '0');
        ms = ms - (minutes * 60000);
        seconds = String(Math.floor(ms/1000)).padStart(2, '0');
    } else {
        result = true;
    }
    const timers = document.getElementsByClassName("next-move-timer");
    for (let timer of timers) {
        timer.innerHTML = `${hours}:${minutes}:${seconds}`;
    }
    return result;
};



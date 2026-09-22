function FormatTimeFromDate(sTime){
    if(!sTime) return "";
    
    return new Date(sTime).toLocaleString("ru").replace(",", "").slice(0, 16)
}

document.querySelectorAll('.deadline').forEach(el => {

    var date = el.textContent;
    var parsedDate = FormatTimeFromDate(date);

    if (new Date(parsedDate) < new Date()) {
        el.style.color = '#ff0000';
    }
    el.textContent = parsedDate;
});

const formContainer = {
    textContainer: document.getElementById("form-project-name"),
}

document.getElementById("form-button-create").addEventListener("click", (e) => {
    var textContainerValue = formContainer.textContainer.value;
    
    if(textContainerValue.trim() == "") { e.preventDefault(); }
})
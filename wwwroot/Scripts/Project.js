const uiElements = {
    newProjectButton: document.getElementById("new-project"),
    formHolder: document.getElementById("form-holder"),
    projectName: document.getElementById("project-name"),
    projectDescription: document.getElementById("project-description"),
    projectSubmitButton: document.getElementById("submit-button"),
    projectCloseButton: document.getElementById("close-form-button")
};

uiElements.newProjectButton.addEventListener("click", () => {
    uiElements.formHolder.style.display = "flex";
})

uiElements.projectSubmitButton.addEventListener("click", (e) => {
    if(uiElements.projectName.value.trim() == ""){
        alert("Project name can't be empty or whitespaces");
        e.preventDefault();
    }
})

uiElements.projectCloseButton.addEventListener("click", () => {
    uiElements.formHolder.style.display = "none";
});
function checkUserInput(e){
    const code = document.getElementById("code").value.trim();

    if(code === ""){
        e.preventDefault();
        alert("Code must be set");
        return;
    }

    if(code.length !== 6){
        e.preventDefault()
        alert("Code must be 6 numbers long")
        return;
    }
}

document.querySelector('form').addEventListener('submit', checkUserInput);
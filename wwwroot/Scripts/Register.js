function checkUserInput(e){
    const username = document.getElementById("username").value.trim();

    if(username === ""){
        e.preventDefault();
        alert("Username must be set");
        return;
    }

    const firstPassword = document.getElementById("password").value.trim();
    const secondPassword = document.getElementById("repeated-password").value.trim();

    if(firstPassword.length < 8) {
        e.preventDefault();
        alert("Password length must be at least 8 symbols.");
        return;
    }

    if(firstPassword !== secondPassword) {
        e.preventDefault();
        alert("Passwords do not match.");
        return;
    }
}

document.querySelector('form').addEventListener('submit', checkUserInput);

function syncHeights(){
    const main = document.querySelector('.main-element');
    const sub = document.getElementById('sub-element');

    if(!main || !sub) return;

    const vw = window.innerWidth || document.documentElement.clientWidth;
    if(vw <= 900) {
        sub.style.height = '';
        return;
    }

    sub.style.height = `${main.getBoundingClientRect().height}px`;
}

window.addEventListener('load', syncHeights);
window.addEventListener('resize', syncHeights);
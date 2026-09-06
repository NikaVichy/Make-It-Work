const inputs = document.querySelectorAll('input.user-input')
inputs.forEach(input => {
    const placeHolder = input.getAttribute('placeholder');
    input.addEventListener('focus', () => input.removeAttribute('placeholder'));
    input.addEventListener('blur', () =>{
        if(input.value.trim() === '') input.setAttribute('placeholder', placeHolder);
    });
});
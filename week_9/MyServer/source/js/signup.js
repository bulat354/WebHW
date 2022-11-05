const name = document.getElementById('name');
const surname = document.getElementById('surname');
const nickname = document.getElementById('nickname');
const birthdate = document.getElementById('birthdate');
const email = document.getElementById('email');
const tel = document.getElementById('tel');
const password = document.getElementById('new-password');
const passwordAccept = document.getElementById('password-accept');
const submitButton = document.getElementById('submit');

let checkValue = function (target, checkFunc) {
    var check = checkFunc(target.value);
    if (check) {
        target.style.background = "var(--valid-color)";
    }
    else {
        target.style.background = "var(--invalid-color)";
    }
    return check;
}
function validate() {
    let bool = checkValue(name, checkName) &
        checkValue(surname, checkName) &
        checkValue(passwordAccept, checkAcceptPass) &
        checkValue(birthdate, checkDate) &
        checkValue(email, checkEmail) &
        checkValue(nickname, checkNick) &
        checkValue(password, checkPass) &
        (checkValue(passwordAccept, checkAcceptPass) &&
        checkValue(passwordAccept, checkPass)) &
        checkValue(tel, checkTel);
    submitButton.disabled = (bool == 0);
    return bool == 1;
}

let checkName = function (text) {
    return text.length > 1 && text.length < 41;
}
let checkDate = function (text) {
    let currentDate = new Date();
    let date = new Date(text);
    return date < currentDate;
}
let checkNick = function (text) {
    let regex = /^[a-z0-9_.]+$/;
    return regex.test(text);
}
let checkEmail = function (text) {
    let regex = /^(([^<>()[\]\.,;:\s@\"]+(\.[^<>()[\]\.,;:\s@\"]+)*)|(\".+\"))@(([^<>()[\]\.,;:\s@\"]+\.)+[^<>()[\]\.,;:\s@\"]{2,})$/i;
    return regex.test(text);
}
let checkTel = function (text) {
    let regex = /^[\+]?[(]?[0-9]{3}[)]?[-\s\.]?[0-9]{3}[-\s\.]?[0-9]{4,6}$/im;
    return regex.test(text);
}
let checkPass = function (text) {
    let regex = /^.*(?=.{8,30})(?=.*[a-zA-Z])(?=.*\d)(?=.*[@!#$%]).*$/;
    return regex.test(text);
}
let checkAcceptPass = function (text) {
    return text == password.value;
}

setInterval(validate, 1000);

const checkboxShow = document.getElementById('show-pass');
checkboxShow.addEventListener('change', function(e) {
    if (e.currentTarget.checked) {
        password.type = 'text';
        passwordAccept.type = 'text';
    }
    else {
        password.type = 'password';
        passwordAccept.type = 'password';
    }
})
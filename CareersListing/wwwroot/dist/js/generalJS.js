
const toggler = document.querySelector(".nav-btn");
const menuBox = document.querySelector(".menu");
const signupBtn = document.querySelector("#signup-btn");
const accBox = document.querySelector("#acc");
const userState = document.querySelector("#user-state");
const loggedInUserOptions = document.querySelector(".loggedIn-user-options");


let flag = false;
let flag1 = false;
let flag2 = false;

toggler.addEventListener("click", function () {
    if (!flag) {
        showMenu(menuBox);
        menuBox.classList.add("fadeInUp");
        flag = true;
    } else {
        hideMenu(menuBox)
        flag = false;
    }
});

signupBtn.addEventListener("click", function () {
    if (!flag1) {
        showMenu(accBox);
        flag1 = true;
    } else {
        hideMenu(accBox)
        flag1 = false;
    }
});

userState.addEventListener("click", function () {
    if (!flag2) {
        showMenu(loggedInUserOptions);
        loggedInUserOptions.classList.add("fadeInUp");
        flag2 = true;
    } else {
        hideMenu(loggedInUserOptions)
        flag2 = false;
    }
})

function showMenu(elem) {
    elem.style.display = "block";
}

function hideMenu(elem) {
    elem.style.display = "none";
}

window.onresize = function () {
    hideMenu(menuBox);
    if (window.innerWidth > 770) {
        showMenu(menuBox);
        menuBox.style.display = "flex";
    }
}

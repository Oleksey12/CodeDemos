const log_tab = document.getElementById("log_tab");
const reg_tab = document.getElementById("reg_tab");

const log_form = document.getElementById("log_form");
const reg_form = document.getElementById("reg_form");

const account_panel = document.getElementById("account");
const enter_panel = document.getElementById("enter");


log_tab.addEventListener('click', function() {
    change_form(log_tab.id);
});

reg_tab.addEventListener('click', function() {
    change_form(reg_tab.id);
});

function change_form(val) {
    if (val == 'log_tab') {
        log_tab.classList.add('is-active');
        reg_tab.classList.remove('is-active');

        log_form.classList.replace('is-hidden', 'is-active');
        reg_form.classList.replace('is-active', 'is-hidden');
    }
    else if(val == 'reg_tab') {
        log_tab.classList.remove('is-active');
        reg_tab.classList.add('is-active');

        log_form.classList.replace('is-active', 'is-hidden');
        reg_form.classList.replace('is-hidden', 'is-active');
    }
}

function manage_session(val) {
    if(val == 1) {
        account_panel.classList.replace('is-hidden', 'is-active');
        enter_panel.classList.replace('is-active', 'is-hidden');
    } else {
        enter_panel.classList.replace('is-hidden', 'is-active');
        account_panel.classList.replace('is-active', 'is-hidden');
    }
}
const insert_tab = document.getElementById("insert_tab");
const edit_tab = document.getElementById("edit_tab");
const delete_tab = document.getElementById("delete_tab");

const insert_form = document.getElementById("insert_form");
const edit_form = document.getElementById("edit_form");
const delete_form = document.getElementById("delete_form");

edit_tab.addEventListener('click', function() {
    change_form(edit_tab.id);
});
insert_tab.addEventListener('click', function() {
    change_form(insert_tab.id);
});
delete_tab.addEventListener('click', function() {
    change_form(delete_tab.id);
});
function change_form(val) {
    if (val == 'insert_tab') {
        insert_tab.classList.add('is-active');
        delete_tab.classList.remove('is-active');
        edit_tab.classList.remove('is-active');

        insert_form.classList.replace('is-hidden', 'is-active');
        delete_form.classList.replace('is-active', 'is-hidden');
        edit_form.classList.replace('is-active', 'is-hidden');
    }
    else if(val == 'delete_tab') {
        delete_tab.classList.add('is-active');
        insert_tab.classList.remove('is-active');
        edit_tab.classList.remove('is-active');

        delete_form.classList.replace('is-hidden', 'is-active');
        insert_form.classList.replace('is-active', 'is-hidden');
        edit_form.classList.replace('is-active', 'is-hidden');
    } else {
        edit_tab.classList.add('is-active');
        insert_tab.classList.remove('is-active');
        delete_tab.classList.remove('is-active');
        
        edit_form.classList.replace('is-hidden', 'is-active');
        insert_form.classList.replace('is-active', 'is-hidden');
        delete_form.classList.replace('is-active', 'is-hidden');
    }
}

const edit_panel =  document.getElementById("admin");
const burgerIcon = document.getElementById("burger");

burgerIcon.addEventListener('click', () => {
    navbarMenu.classList.toggle("is-active");
});

function hide_tab(active, privilege) {
    if(active != 1 || privilege != "admin") {
        edit_panel.classList.add("is-hidden");
    }
}





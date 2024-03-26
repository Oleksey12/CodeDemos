const students = document.getElementById("students");
const school_select = document.getElementById("school_select");
const navbar = document.getElementById("navbar-text");

let previous_school = school_select.value;

school_select.addEventListener('change', function() {
    set(school_select.value);
});

onStart();
function onStart(){
    student_db.forEach((value,key) => {
        if(value.school == school_select.value) {
            let newDiv = document.createElement("div");
            newDiv.classList.add("column");
            newDiv.classList.add("is-3-desktop");
            newDiv.classList.add("is-4-tablet");
            newDiv.classList.add("is-6-mobile");
            newDiv.classList.add("py-4");
            newDiv.appendChild(createCard(value));

            students.appendChild(newDiv);
        }
    })
}


function update() {
    students.textContent='';
    var elements = document.querySelectorAll(".card_input");
    elements.forEach((elm) => {
        elm.classList.replace(previous_school.concat("-card"), school_select.value.concat("-card"));
        console.log(elm.classList);
    })

    elements = document.querySelectorAll(".card_button");
    elements.forEach((elm) => {
        elm.classList.replace(previous_school.concat("-color"), school_select.value.concat("-color"));
        console.log(elm.classList);
    });

    previous_school = school_select.value;
    students.textContent='';
    onStart();
}

function set(val) {
    school_select.value=val;
    if(val=="YouTube") {
        navbar.style="background-color: rgb(255, 104, 104)";
    } else {
        navbar.style="background-color: rgb(167, 139, 228)";
    }
    update();
}



function createCard(value) {
    let newDiv = document.createElement("div");

    newDiv.innerHTML = `
        <div class="card ${value.school}-card">
            <div class="card-header ${value.school}-bg" style="height: 30px"></div> 
            <div class="card-image has-text-centered px-6">
                <img src="assets/${value.image_name}" style="width: 100%;">
            </div>
            <div class="card-content has-text-centered">
                <label class="is-size-3 has-text-weight-bold"> ${value.Name}</label>
                <h2 class="is-size-3">
                    ${value.education_year} "${value.class_letter}"
                </h2>
            </div>
        </div>
    </div>
    `;

    return newDiv;
}


function restoreSearchValues(name, grade, letter) {
    const _name = document.getElementById("name");
    const _grade = document.getElementById("grade");
    const _letter = document.getElementById("letter");

    _name.value = name;
    _grade.value = grade;
    _letter.value = letter;
}



String.prototype.replaceAt = function(index, replacement) {
    return this.substring(0, index) + replacement + this.substring(index + replacement.length);
}
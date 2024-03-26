let student_db = [

];

// Помещает строку значений в локальную базу данных
function stringToObject(studentStr) {
    var studentArray = studentStr.split('|');
    var baseArray = { 
        Name: "",
        image_name: "",
        education_year: 0,
        class_letter: "",
        school: ""
    }


    for(var i = 0; i < studentArray.length/5; ++i) {
        let tempArray = {
            ...baseArray
        };
        tempArray.Name = studentArray[5*i];
        tempArray.image_name = studentArray[5*i+1];
        tempArray.education_year = parseInt(studentArray[5*i+2]);
        tempArray.class_letter = studentArray[5*i+3];
        tempArray.school = studentArray[5*i+4];
        student_db.push(tempArray);
    }

}
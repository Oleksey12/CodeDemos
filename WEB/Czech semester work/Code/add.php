<?php
  require_once('session.php');
  require_once('config/connect.php');

  $submit_class = $_GET['submit_class'];
  $submit_student = $_GET['submit_student'];

  $ed_class = $_GET['ed_class'];
  $ed_student = $_GET['ed_student'];

  $del_class = $_GET['del_class'];
  $del_student = $_GET['del_student'];
  
  if (isset($submit_class)) {
    $id = $_GET['id'];
    $class_year = $_GET['class_year'];
    $class_letter = $_GET['class_letter'];
    $class_teacher = $_GET['class_teacher'];
    $school = $_GET['school'];


    $insert_que = "INSERT INTO Class(class_id, education_year, class_letter, school, teacher) VALUES ($id, $class_year, '$class_letter', '$school', '$class_teacher')";
    if (mysqli_query($connect, $insert_que)) {
      echo "Added 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }

  if (isset($submit_student)) {
    $student_name = $_GET['student_name'];
    $class_id = $_GET['class_id'];
    $student_description = $_GET['student_description'];
    $student_image = $_GET['student_image'];


    $insert_que = "INSERT INTO Student(Name, class_id, description, image_name) VALUES ('$student_name', $class_id, '$student_description', '$student_image')";
    if (mysqli_query($connect, $insert_que)) {
      echo "Added 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }

  if (isset($ed_class)) {
    $edit_class= $_GET['edit_class'];
    $id = explode(' ', $edit_class)[0];

    $class_id = $_GET['e_id'];
    $class_year = $_GET['e_class_year'];
    $class_letter = $_GET['e_class_letter'];
    $class_teacher = $_GET['e_class_teacher'];
    $school = $_GET['e_school'];


    $update_que = "UPDATE Class SET class_id=$class_id, education_year = $class_year, class_letter = '$class_letter', school = '$school', teacher = '$class_teacher' WHERE Class.class_id = $id";
    if (mysqli_query($connect, $update_que)) {
      echo "Edited 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }

  if (isset($ed_student)) {
    $edit_student = $_GET['$edit_student'];
    $id = explode(' ', $edit_student)[0];

    $student_name = $_GET['e_student_name'];
    $class_id = $_GET['e_class_id'];
    $student_description = $_GET['e_student_description'];
    $student_image = $_GET['e_student_image'];


    $update_que = "UPDATE Student SET Name = '$student_name', class_id = $class_id, description = '$student_description', image_name = '$student_image' WHERE Student.Name = '$student_name'";
    if (mysqli_query($connect, $update_que)) {
      echo "Edited 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }

  if (isset($del_class)) {
    $delete_class = $_GET['delete_class'];
    $id = explode(' ', $delete_class)[0];
    $delete_que = "DELETE FROM Class WHERE Class.class_id = $id";

    if (mysqli_query($connect, $delete_que)) {
      echo "Removed 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }

  if (isset($del_student)) {
    $delete_student = $_GET['delete_student'];
    $id = explode(' ', $delete_student)[0];
    
    $delete_que = "DELETE FROM Student WHERE Class.Name = '$id'";
    if (mysqli_query($connect, $delete_que)) {
      echo "Removed 1 element succesfully";
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }


  $find_max_query = "SELECT MAX(class_id) FROM Class";

  $query_result = mysqli_query($connect, $find_max_query);
  $max_number = mysqli_fetch_all($query_result);
  $max_number = implode($max_number[0]);
  
  $max_number = (string)((int)$max_number + 1);


  $class_record = "SELECT * FROM Class";
  $student_record = "SELECT * FROM Student";

  $class_edit_record = mysqli_query($connect, $class_record);
  $student_edit_record = mysqli_query($connect, $student_record);

  $class_delete_record = mysqli_query($connect, $class_record);
  $student_delete_record = mysqli_query($connect, $student_record);
?>


<!DOCTYPE html>
<html>
  <head>
    <meta charset="utf-8">
    <meta name="viewport" content="width=device-width, initial-scale=1">
    <title>Students</title>
    <link rel="stylesheet" href="bg.css">
    <link rel="stylesheet" href="bulma/css/bulma.css">
  </head>

  <body class="has-background-warning-light has-text-black">
  <nav class="navbar has-shadow is-transparent" style="background: linear-gradient(to right, rgb(255, 104, 104) 50%, rgb(167, 139, 228) 50%);" id="navbar-text">
      <div class="navbar-menu" id="nav-links">
        <div class="navbar-start is-dekstop-only">
          <a class="navbar-item is-size-4 has-text-warning-light" href="index.php"> News </a>
          <a class="navbar-item is-size-4 has-text-warning-light" href="list.php"> Students </a>
        </div>
      </div>
      <div class="navbar-brand">
        <a class="navbar-item has-text-weight-bold is-size-3 has-text-warning-light" href="index.php"> Education center for streamers  №12 </a>
        <a class="navbar-burger" id="burger">
            <span></span>
            <span></span>
            <span></span>
        </a>   
      </div>
      <div class="navbar-menu" id="nav-links">
        <div class="navbar-end">
            <a class="navbar-item is-size-4 has-text-warning-light" href="add.php">Edit</a>
            <div class="navbar-item">
                <a id="profile" href="login.php">
                    <img src="assets/profile.svg" alt = "profile" class="navbar-item has-text-warning-light" style="max-height: 100px" id="profilepic">
                </a>
            </div>
        </div>
    </div>
  </nav>

  <div class="section">
    <div class="container">
      <article class="panel is-primary">
        <p class="panel-heading has-text-warning-light">
          Admin panel
        </p>
        <p class="panel-tabs is-large">
          <a class="is-active" id="insert_tab">Insert</a>
          <a class="" id="edit_tab">Edit</a>
          <a class="" id="delete_tab">Delete</a>
        </p>

        <div class="section is-active" id="insert_form">
          <div class="container">
            <form action="" class="is-size-5" id="insertForm" method="GET">
              <label class="is-size-3 has-text-weight-bold">Add new class</label>
              <table class="is-size-4">
                <tr><td><label style="padding-right: 80px;">ID</label><input class="is-size-5" name="id" id="id" style="width: 70px;" value=<?php echo $max_number ?> required></td>
                </tr>
                <tr><td><label style="padding-right: 60px;">Year</label><input class="is-size-5" name="class_year" id="class_year" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 44px;">Letter</label><input class="is-size-5" name="class_letter" id="class_letter" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 25px;">Teacher</label><input class="is-size-5" name="class_teacher" id="class_teacher" style="width: 120px;" value="NULL"></td>
                </tr>
                <tr><td><label style="padding-right: 35px;">School</label><input class="is-size-5" name="school" id="school" style="width: 120px;" value="YouTube" required></td>
                </tr>
                <tr><td><input class="is-size-5" type="submit" name="submit_class" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>

            <br><br>

            <form action="" class="is-size-5" id="insertForm" method="GET">
              <div class="is-size-3 has-text-weight-bold">Add new student</div>
              <table class="is-size-4">
                <tr><td><label style="padding-right: 87px;">Name</label><input class="is-size-5" name="student_name" id="student_name" style="width: 200px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 67px;">Class_id</label><input class="is-size-5" name="class_id" id="class_id" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 30px;">Description</label><input class="is-size-5" name="student_description" id="student_description" style="width: 200px;" value="NULL"></td>
                </tr>
                <tr><td><label style="padding-right: 17px;">Image name</label><input class="is-size-5" name="student_image" id="student_image" style="width: 200px;" required></td>
                </tr>
                
                <tr><td><input class="is-size-5" type="submit" name="submit_student" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>
          </div>
        </div>

        <div class="section is-hidden" id="edit_form">
          <div class="container">
            <form action="" class="is-size-5" method="GET">
              <label class="is-size-3 has-text-weight-bold">Edit class</label>
              <table class="is-size-4">
              <tr><td><label style="padding-right: 22px;">Record</label>
                <select class="is-size-5" name="edit_class" id="edit_class" style="width: 150px" data-dropup-auto="false">
                <?php
                  while ($record = mysqli_fetch_assoc($class_edit_record)) {
                    $optionstr = $record['class_id']." ".$record['education_year']." ".$record['class_letter']." ".$record['school'];
                    echo "<option>".$optionstr."</option>";
                  }         
                ?>
                </select></td>
                </tr>
                <tr><td><label style="padding-right: 80px;">ID</label><input class="is-size-5" name="e_id" id="e_id" style="width: 70px;" value=<?php echo $max_number ?> required></td>
                </tr>
                <tr><td><label style="padding-right: 60px;">Year</label><input class="is-size-5" name="e_class_year" id="e_class_year" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 44px;">Letter</label><input class="is-size-5" name="e_class_letter" id="e_class_letter" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 25px;">Teacher</label><input class="is-size-5" name="e_class_teacher" id="e_class_teacher" style="width: 120px;" value="NULL"></td>
                </tr>
                <tr><td><label style="padding-right: 35px;">School</label><input class="is-size-5" name="e_school" id="e_school" style="width: 120px;" value="YouTube" required></td>
                </tr>
                <tr><td><input class="is-size-5" type="submit" name="ed_class" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>

            <br><br>

            <form action="" class="is-size-5" method="GET">
              <div class="is-size-3 has-text-weight-bold">Edit student</div>
              <table class="is-size-4">
                <tr><td><label style="padding-right: 70px;">Record</label>
                <select class="is-size-5" name="edit_student" id="edit_student" style="width: 150px" data-dropup-auto="false">
                  <?php
                    while ($record = mysqli_fetch_assoc($student_edit_record)) {
                      $optionstr = $record["Name"]." ".$record["class_id"];
                      echo "<option>".$optionstr."</option>";
                    }
                  ?>
                </select></td>
                <tr><td><label style="padding-right: 87px;">Name</label><input class="is-size-5" name="e_student_name" id="e_student_name" style="width: 200px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 67px;">Class_id</label><input class="is-size-5" name="e_class_id" id="e_class_id" style="width: 70px;" required></td>
                </tr>
                <tr><td><label style="padding-right: 30px;">Description</label><input class="is-size-5" name="e_student_description" id="e_student_description" style="width: 200px;" value="NULL"></td>
                </tr>
                <tr><td><label style="padding-right: 17px;">Image name</label><input class="is-size-5" name="e_student_image" id="e_student_image" style="width: 200px;" required></td>
                </tr>
                </tr>
                <tr><td><input class="is-size-5" type="submit" name="ed_student" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>
          </div>
        </div>

        <div class="section is-hidden" id="delete_form">
          <div class="container">
            <form action="" class="is-size-5" method="GET">
              <label class="is-size-3 has-text-weight-bold">Delete class</label>
              <table class="is-size-4">
              <tr><td><label style="padding-right: 40px;">Record</label>
                <select class="is-size-5" name="delete_class" id="delete_class" style="width: 150px" data-dropup-auto="false">
                <?php
                  while ($record = mysqli_fetch_assoc($class_delete_record)) {
                    $optionstr = $record['class_id']." ".$record['education_year']." ".$record['class_letter']." ".$record['school'];
                    echo "<option>".$optionstr."</option>";
                  }         
                ?>
                </select></td>
                </tr>
                <tr><td><input class="is-size-5" type="submit" name="del_class" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>

            <br><br>

            <form action="" class="is-size-5" method="GET">
              <div class="is-size-3 has-text-weight-bold">Delete student</div>
              <table class="is-size-4">
                <tr><td><label style="padding-right: 40px;">Record</label>
                <select class="is-size-5" name="delete_student" id="delete_student" style="width: 150px" data-dropup-auto="false">
                  <?php
                    while ($record = mysqli_fetch_assoc($student_delete_record)) {
                      $optionstr = $record["Name"]." ".$record["class_id"];
                      echo "<option>".$optionstr."</option>";
                    }
                  ?>
                </select></td>
                </tr>
                <tr><td><input class="is-size-5" type="submit" name="del_student" style="width: 100px" value="Send"></td></tr>
              </table>
            </form>
          </div>
        </div>
        <script type="text/javascript" src="admin.js"></script>
      </article>
    </div>
  </div>

    <!--Отступы для красоты-->
  <section class="section">
      <div class="container py-6 my-6">
      </div>
      <div class="container py-6 my-6">
      </div>
      <div class="container py-6 my-6">
      </div>
  </section>
  <footer class="footer has-background-grey-lighter">
    <div class="has-text-centered">
        <div class="is-size-5">Copyright 2023 Streamer site</div>
    </div>
  </footer>
</body>
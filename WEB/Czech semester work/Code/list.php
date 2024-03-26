<?php
    require_once('session.php');
    require_once('config/connect.php');

    $submit = $_GET['submit'];
    $schl = $_GET['school'];
    $n = $_GET['name'];
    $grd = $_GET['grade'];
    $let = $_GET['letter'];

    //Строки для запросов в бд
    $school = $schl;
    if (!isset($school) || $school == "")
      $school = "YouTube";

    $nme = $n;
    if (!isset($nme) || $nme == "")
      $nme = "%";
    else
      $nme = "%".$nme."%";

    $grade = $grd;
    if (!isset($grade) || $grade == "")
      $grade = "%";

    $letter = $let;
    if (!isset($letter) || $letter == "")
      $letter = "%";

    $students = "SELECT Student.Name, Student.image_name, Class.education_year, Class.class_letter, School.name 
    FROM Student INNER JOIN Class ON Student.class_id=Class.class_id 
    INNER JOIN School ON Class.school=School.name
    WHERE Student.Name LIKE '$nme' AND Class.education_year LIKE '$grade' AND Class.class_letter LIKE '$letter'";

    $students_data = mysqli_query($connect, $students);
    $students_data = mysqli_fetch_all($students_data);

    $sendedStr ="";
        
    foreach($students_data as $item) {
        if($sendedStr != "")
            $sendedStr .= '|';

        $sendedStr .= implode('|', $item);
    }

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
    <!--NavBar-->
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
            <a class="navbar-item is-size-4 has-text-warning-light" id="admin" href="add.php">Edit</a>
            <div class="navbar-item">
                <a id="profile" href="login.php">
                    <img src="assets/profile.svg" alt = "profile" class="navbar-item has-text-warning-light" style="max-height: 100px" id="profilepic">
                </a>
            </div>
        </div>
    </div>
  </nav>


    <!--Search-->
    <div class="section">
      <div class="container">
        <div class="is-size-2 has-text-weight-bold has-text-black">Our beloved pupils:</div>
        <br>
        <div class="is-size-3 has-text-weight-bold">Search student</div>
        <form action="" class="is-size-5" id="myForm" method="GET">
          <img class="is-size-5" src="assets/search.svg" style="max-height: 30px;" alt="Search">
          <input class="is-size-5 card_input YouTube-card" type="text" name="name" id="name" value="">
          
          <select class="is-size-5 card_button card_input YouTube-card YouTube-color has-text-warning-light" name="school" id="school_select" style="width: 110px" value="YouTube">
            <option> YouTube </option>
            <option> Twitch </option>
          </select>

          <input class="is-size-5 card_input YouTube-card" type="text" name="grade" id="grade" style="width: 40px" value="">
          <input class="is-size-5 card_input YouTube-card" type="text" name="letter" id="letter" style="width: 40px" value="">
          
          
          <input class="is-size-5" type="submit" name="submit" style="width: 80px" value="Search">
        </form>
      </div>
    </div>



    <!--Students-->
    <div class="section">
      <div class="container">
          <div class="columns is-multiline is-mobile" id="students">

          </dev>
      </div>
    </div>


    <script type="text/javascript" src="db.js"></script>
    <script type="text/javascript"> 
        stringToObject(<?php echo json_encode($sendedStr, JSON_HEX_TAG); ?>); 
    </script>
    <script type="text/javascript" src="student.js"></script>
    <script type="text/javascript"> 
        set(<?php echo json_encode($school, JSON_HEX_TAG); ?>); 
        restoreSearchValues(<?php echo json_encode($n, JSON_HEX_TAG); ?>, 
                            <?php echo json_encode($grd, JSON_HEX_TAG); ?>, 
                            <?php echo json_encode($let, JSON_HEX_TAG); ?>); 
    </script>
    <script type="text/javascript" src="interface.js"></script>
    <script type="text/javascript">
    hide_tab(<?php echo json_encode($_SESSION["user_is_logged"], JSON_HEX_TAG); ?>,<?php echo json_encode($_SESSION["privilege"], JSON_HEX_TAG); ?>);
  </script>


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
</html>
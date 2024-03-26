<?php
  require_once('session.php');
  require_once('config/connect.php');
  if (isset($_GET['log_submit'])) {
    $user = $_POST['log_login'];
    $pass = hash("sha256", $_POST['log_password']);
    $sql = "select * from User where login='$user' AND password='$pass'";
    $response = mysqli_query($connect, $sql);
    if (mysqli_num_rows($response) > 0) {
      echo "Successful login!";
      $record = mysqli_fetch_assoc($response);
      session_start();
      header("Cache-control: private");
      $_SESSION["user_is_logged"] = 1;
      $_SESSION["privilege"] = $record['privilege'];
      $_SESSION["login"] = $record['login'];
      header("location: login.php");
    }
    else {
      echo "Error wrong login or password!";
    }
  }
  else if (isset($_GET['reg_submit'])) {
    $user = $_POST['reg_login'];
    $pass = hash("sha256", $_POST['reg_password']);

    $sql = "INSERT INTO User(login, password, privilege) VALUES ('$user', '$pass', 'user')";

    if (mysqli_query($connect, $sql)) {
      echo "You have registered succesfully";
      session_start();
      header("Cache-control: private");
      $_SESSION["user_is_logged"] = 1;
      $_SESSION["privilege"] = 'user';
      $_SESSION["login"] = $user;
      header("location: login.php");
    } else {
      echo "Error: ". mysqli_error($connect);
    }
  }
  else if (isset($_GET['logout_submit'])) {
    $_SESSION["user_is_logged"] = 0;
    session_destroy();
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

  <div class="section is-hidden" id="account">
    <div class="container">
      <article class="panel is-primary">
          <p class="panel-heading has-text-warning-light">
            Account
          </p>

        <div class="section is-active" id="log">
          <div class="container">
            <form action="" class="is-size-5" method="GET">
              <div class="columns is-centered">
                <div class="columns is-half">
                  <div class="rows">
                    <div class="row">
                      <label class="is-size-4 label">Entered as</label> 
                    </div>
                    <div class="row">
                      <label class="is-size-4">Login:</label> 
                      <label class="is-size-4"> <?php echo $_SESSION["login"]; ?> </label>
                    </div>
                    <div class="row">
                      <label class="is-size-4"> Privilege:</label>
                      <label class="is-size-4"> <?php echo $_SESSION["privilege"]; ?> </label>
                    </div>
                    <div class="row">
                      <input class="is-size-4 button" type="submit" name="logout_submit" style="margin-top: 5px; height: 60px;" value="Logout">
                    </div>
                  </div>
                </div>
              </div>  
            </form>
          </div>
        </div>
      </article>
    </div>
  </div>

  <div class="section is-active" id="enter">
    <div class="container">
      <article class="panel is-primary">
        <p class="panel-heading has-text-warning-light">
          Account
        </p>
        <p class="panel-tabs">
          <a class="is-active" id="log_tab">Login</a>
          <a class="" id="reg_tab">Register</a>
        </p>

      <div class="section is-active" id="log_form">
        <div class="container">
          <form action="./login.php?log_submit=Enter" class="is-size-5" method="POST">
            <div class="columns is-centered">
              <div class="columns is-half">
                <div class="rows">
                  <div class="row">
                    <label class="is-size-5 has-text-grey">(test123 test123)</label>
                  </div>
                  <div class="row">
                    <label class="is-size-4 label">Login</label>
                  </div>
                  <div class="row">
                    <input class="is-size-5 input" name="log_login" id="log_login" style="width: 200px; height: 40px;" required>
                  </div>
                  <div class="row">
                    <label class="is-size-4 label">Password</label>
                  </div>
                  <div class="row">
                    <input class="is-size-5 input" type="password" name="log_password" id="log_password" style="width: 200px; height: 40px;" required>
                  </div>
                  <div class="row">
                      <input class="is-size-4 button" type="submit" name="log_submit" style="margin-top: 5px; height: 60px;" value="Enter">
                  </div>
                </div>
              </div>
            </div>  
          </form>
        </div>
      </div>

      <div class="section is-hidden" id="reg_form">
        <div class="container">
          <form action="./login.php?reg_submit=Register" class="is-size-5" method="POST">
            <div class="columns is-centered">
              <div class="columns is-half">
                <div class="rows">
                  <div class="row">
                    <label class="is-size-4 label">Login</label>
                  </div>
                  <div class="row">
                    <input class="is-size-5 input" name="reg_login" id="reg_login" style="width: 200px; height: 35px;" required>
                  </div>
                  <div class="row">
                    <label class="is-size-4 label">Password</label>
                  </div>
                  <div class="row">
                    <input class="is-size-5 input" type="password" name="reg_password" id="reg_password"  style="width: 200px; height: 35px;" required>
                  </div>
                  <div class="row">
                    <label class="is-size-5 checkbox" style="margin-top: 6px">
                      <input type="checkbox" id="reg_checkbox" style="width: 17px; height: 17px;" required>
                      I agree to <a href="https://www.twitch.tv/p/en/legal/terms-of-service/">rules and terms</a>
                    </label>
                  </div>
                  <div class="row">
                      <input class="is-size-4 button" type="submit" name="reg_submit" style="margin-top: 4px; height: 60px;" value="Register">
                  </div>
                </div>
              </div>
            </div>  
          </form>
        </div>
      </div>
    </article>
    </div>
  </div>

  <script type="text/javascript" src="login.js"></script>
  <script type="text/javascript">
    manage_session(<?php echo json_encode($_SESSION["user_is_logged"], JSON_HEX_TAG); ?>);
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
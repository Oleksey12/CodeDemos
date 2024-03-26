<?php
  require_once('session.php');
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
            <a class="navbar-item is-size-4 has-text-warning-light" id="admin" href="add.php">Edit</a>
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
      <div class="is-size-2 has-text-black has-text-weight-bold">School news</div>
      <br>
      <div>
        <div class="rows">
          <div class="row">
            <div class="is-size-3 has-text-black">News1</div>
            <div class="columns">
              <div class="column is-7">
                <p class="is-size-5 has-text-black"> Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and f Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions </p>
              </div>
              <div class="column is-5">
                  <img src="assets/Stas.png" alt="News1">;
              </div>
            </div>
          </div>
          
          <br><br><br>
          <div class="row">
            <div class="is-size-3 has-text-black">News2</div>
            <div class="columns">
              <div class="column is-7">
                <p class="is-size-5 has-text-black"> Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and f Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions </p>
              </div>
              <div class="column is-5">
                  <img src="assets/Stas.png" alt="News1">;
              </div>
            </div>
          </div>

          <br><br><br>
          <div class="row">
            <div class="is-size-3 has-text-black">News3</div>
            <div class="columns">
              <div class="column is-7">
                <p class="is-size-5 has-text-black"> Lorem Ipsum is simply dummy text of the printing and typesetting industry. Lorem Ipsum has been the industry's standard dummy text ever since the 1500s, when an unknown printer took a galley of type and f Letraset sheets containing Lorem Ipsum passages, and more recently with desktop publishing software like Aldus PageMaker including versions </p>
              </div>
              <div class="column is-5">
                  <img src="assets/Stas.png" alt="News1">;
              </div>
            </div>
          </div>

        </div>
      </div>
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

  <script type="text/javascript" src="interface.js"></script>
  <script type="text/javascript">
    hide_tab(<?php echo json_encode($_SESSION["user_is_logged"], JSON_HEX_TAG); ?>,<?php echo json_encode($_SESSION["privilege"], JSON_HEX_TAG); ?>);
  </script>
</body>
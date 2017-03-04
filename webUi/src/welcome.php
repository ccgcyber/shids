<html>
    <head>
        <?php include_once 'header.inc.php'; ?>
        <script>
            $(function() {
                $("#dialog").dialog({
                    closeOnEscape: false,
                    open: function(event, ui) {
                        $(".ui-dialog-titlebar-close", ui.dialog || ui).hide();
                    }
                });
            });
        </script>
    </head>
    <body>
        <div id="dialog" title="Welcome to S|HIDS" style="width: 700px;height: 500px;">
            <p>This is S|Hids <br/>(Simple Host incident detection system).</p>
            <p>This is the <?php echo VERSION;?> release</p>
            <p>This application is <br/>developed by :</p>
            <ul>
                <li>Ruwan</li>                
            </ul>
        </div>
    </body>
</html>
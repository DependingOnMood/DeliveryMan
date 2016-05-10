
<html xmlns="http://www.w3.org/1999/xhtml">
<head>
<title>Get User Current Location using Google Map Geolocation API Service in asp.net website</title>
<style type="text/css">
html { height: 100% }
body { height: 100%; margin: 0; padding: 0 }
#map_canvas { height: 100% }
</style>
<script type="text/javascript"src="https://maps.googleapis.com/maps/api/js?key=AIzaSyC6v5-2uaq_wusHDktM9ILcqIrlPtnZgEk&sensor=false">
</script>
<script type="text/javascript" src="http://maps.googleapis.com/maps/api/js?sensor=false&libraries=places">
</script>
<script type="text/javascript">
    function findMyLocation() {
        if (navigator.geolocation) {
            navigator.geolocation.getCurrentPosition(success);
        } else {
            alert("Geo Location is not supported on your current browser!");
        }
        function success(position) {
            var lat = position.coords.latitude;
            var long = position.coords.longitude;
            var city = position.coords.locality;

            dvDistance.innerHTML += "lat: " + lat + " long: " + long + "<br />";
        }
    }
</script>
</head>
<body onload ="findMyLocation()">
<form id="form1" runat="server">
<div id="map_canvas" style="width: 500px; height: 400px"></div>
        <div id="dvDistance"></div>
</form>
</body>
</html>
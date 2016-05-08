using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Xml.Linq;

namespace BizLogic
{
    public class GoogleMapHelper
    {
        //Given address information returns latitude and longitude
        public String getLatandLngByAddr(String addr)
        {

            var address = addr;
            //var address = "110 Riverdrive south";
            var requestUri = string.Format("http://maps.googleapis.com/maps/api/geocode/xml?address={0}&sensor=false", Uri.EscapeDataString(address));

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var result = xdoc.Element("GeocodeResponse").Element("result");
            var locationElement = result.Element("geometry").Element("location");
            var lat = locationElement.Element("lat");
            var lng = locationElement.Element("lng");
            String[] res1 = lat.ToString().Split('<');
            String[] res2 = lng.ToString().Split('<');
            String _lat = res1[1].Substring(4);
            String _lng = res2[1].Substring(4);
            return _lat + " " + _lng;
        }

        public String getRoute(String addr1, String addr2)
        {
            var requestUri = string.Format("https://maps.googleapis.com/maps/api/directions/xml?origin={0}&destination={1}", addr1, addr2);
            //var requestUri = "https://maps.googleapis.com/maps/api/directions/xml?origin=110%20riverdrive%20south&destination=New%20York%20University";

            var request = WebRequest.Create(requestUri);
            var response = request.GetResponse();
            var xdoc = XDocument.Load(response.GetResponseStream());

            var duration = xdoc.Element("DirectionsResponse").Element("route").Element("leg").Element("duration");
            var distance = xdoc.Element("DirectionsResponse").Element("route").Element("leg").Element("distance");

            String duValue = duration.Element("value").ToString().Split('<')[1].Substring(6);
            String duText = duration.Element("text").ToString().Split('<')[1].Substring(5);
            String disValue = distance.Element("value").ToString().Split('<')[1].Substring(6);
            String disText = distance.Element("text").ToString().Split('<')[1].Substring(5);
            String res = duValue + "#" + duText + "#" + disValue + "#" + disText;
            //String res = 
            return res;
        }

        //Compute distance between two locations by given latitude and longitude pairs
        public double computeDistanceByLocation(String loc1, String loc2)
        {

            double lat1 = Double.Parse(loc1.Split(' ')[0]);
            double lon1 = Double.Parse(loc1.Split(' ')[1]);
            double lat2 = Double.Parse(loc2.Split(' ')[0]);
            double lon2 = Double.Parse(loc2.Split(' ')[1]);

            double theta = lon1 - lon2;
            double dist = Math.Sin(deg2rad(lat1)) * Math.Sin(deg2rad(lat2)) + Math.Cos(deg2rad(lat1)) * Math.Cos(deg2rad(lat2)) * Math.Cos(deg2rad(theta));
            dist = Math.Acos(dist);
            dist = rad2deg(dist);
            dist = dist * 60 * 1.1515;
            return dist;
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts decimal degrees to radians             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double deg2rad(double deg)
        {
            return (deg * Math.PI / 180.0);
        }

        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        //::  This function converts radians to decimal degrees             :::
        //:::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::::
        private double rad2deg(double rad)
        {
            return (rad / Math.PI * 180.0);
        }
     
    }
}
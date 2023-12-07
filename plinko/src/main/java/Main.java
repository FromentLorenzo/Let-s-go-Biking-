import com.soap.ws.client.generated.GetRoute;
import com.soap.ws.client.generated.Service1;

import javax.swing.JFrame;

import mapUtilities.Itineraire;
import org.jxmapviewer.JXMapKit;
import org.jxmapviewer.JXMapViewer;
import org.jxmapviewer.OSMTileFactoryInfo;
import org.jxmapviewer.viewer.*;

import org.jxmapviewer.painter.CompoundPainter;
import org.jxmapviewer.painter.Painter;

import java.awt.*;
import java.util.*;
import java.util.List;

import static mapUtilities.Itineraire.*;


public class Main {

    public static void main(String[] args) {
        Scanner scanner = new Scanner(System.in);

        Service1 service1 = new Service1();
        GetRoute getRoute = new GetRoute();

        System.out.println("where are you ? ");
        String depart = scanner.nextLine();
        System.out.println("Where do you want to go ? ");
        String arrivee = scanner.nextLine();

        String route = service1.getBasicHttpBindingIService1().getRoute(depart, arrivee);

        // authorized adresses :
        // rue paul bert, lyon
        // rue des rancy, lyon

        List<String> itineraries = Arrays.asList(route.split("\n\n"));

        ArrayList<GeoPosition> walk1 = new ArrayList<>();
        ArrayList<GeoPosition> bike = new ArrayList<>();
        ArrayList<GeoPosition> walk2 = new ArrayList<>();
        if (itineraries.size() == 6) {
            walk1 = parseCoordinates(itineraries.get(1));
            bike = parseCoordinates(itineraries.get(3));
            walk2 = parseCoordinates(itineraries.get(5));
        }

        ArrayList<GeoPosition> allInstructions = new ArrayList<>();
        allInstructions.addAll(walk1);
        allInstructions.addAll(bike);
        allInstructions.addAll(walk2);

        JXMapViewer mapViewer = new JXMapKit().getMainMap();


        // Create a TileFactoryInfo for OpenStreetMap
        TileFactoryInfo info = new OSMTileFactoryInfo();
        DefaultTileFactory tileFactory = new DefaultTileFactory(info);
        mapViewer.setTileFactory(tileFactory);

        // Use 8 threads in parallel to load the tiles
        tileFactory.setThreadPoolSize(8);

        try {
            mapViewer.zoomToBestFit(new HashSet<>(allInstructions), 0.5);
        } catch (Exception e) {
            mapViewer.setZoom(10);
        }


        mapViewer.setInfiniteMapRendering(true);
        mapViewer.setPanEnabled(true);

        // Display the viewer in a JFrame
        JFrame frame = new JFrame("JXMapviewer2 Example 1");
        frame.getContentPane().add(mapViewer);
        frame.setSize(800, 600);
        frame.setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        frame.setVisible(true);


        Set<Waypoint> waypoints = new HashSet<>();
        for (GeoPosition position : allInstructions) {
            waypoints.add(new DefaultWaypoint(position));
        }


        Itineraire itineraire = new Itineraire(allInstructions);
        displayItineraire(mapViewer, itineraire);


        // Création du WaypointPainter
        WaypointPainter<Waypoint> allInstructionsPainter = new WaypointPainter<>();

        allInstructionsPainter.setWaypoints(waypoints);

        // Création du CompoundPainter
        CompoundPainter<JXMapViewer> compoundPainter = new CompoundPainter<>();

        // Ajout du CompoundPainter à la carte
//        mapViewer.setOverlayPainter(compoundPainter);



    }


    static ArrayList<GeoPosition> parseCoordinates(String input) {
        ArrayList<GeoPosition> coordinatesList = new ArrayList<>();

        String[] lines = input.split("\n");
        for (String line : lines) {
            String[] parts = line.split("-|,");
            if (parts.length == 4) {
                double latitude = Double.parseDouble(parts[0] + "." + parts[1]);
                double longitude = Double.parseDouble(parts[2] + "." + parts[3]); // Combine les parties décimales
                coordinatesList.add(new GeoPosition(latitude, longitude));
            }
        }

        return coordinatesList;
    }





}
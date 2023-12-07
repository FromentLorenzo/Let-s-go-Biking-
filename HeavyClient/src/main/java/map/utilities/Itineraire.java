package map.utilities;

import org.jxmapviewer.JXMapViewer;
import org.jxmapviewer.painter.CompoundPainter;
import org.jxmapviewer.viewer.*;
import org.jxmapviewer.painter.Painter;

import javax.persistence.Subgraph;
import javax.persistence.Tuple;
import javax.swing.*;
import java.awt.*;
import java.awt.geom.Point2D;
import java.util.*;
import java.util.List;

public class Itineraire {
    private List<GeoPosition> points;

    public Itineraire(List<GeoPosition> points) {
        this.points = new ArrayList<>(points);
    }

    public List<GeoPosition> getpoints() {
        return points;
    }

    static void displayItineraire(JXMapViewer mapViewer, Itineraire itineraire) {

        // on ajoute un Waypooint painter pour afficher les points de l'itinÃ©raire
        Set<Waypoint> waypoints = new HashSet<>();
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(0)));
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(itineraire.getpoints().size() - 1)));

        WaypointPainter<Waypoint> waypointPainter = new WaypointPainter<>();
        waypointPainter.setWaypoints(waypoints);

        Painter<JXMapViewer> itinerairePainter = new ItinerairePainter(itineraire.getpoints());

        List<Painter<JXMapViewer>> painters = new ArrayList<>();
        painters.add(itinerairePainter);
        painters.add(waypointPainter);

        CompoundPainter<JXMapViewer> painter = new CompoundPainter<>(painters);
        mapViewer.setOverlayPainter(painter);
    }

    private static class ItinerairePainter implements Painter<JXMapViewer> {
        private final List<GeoPosition> itineraire;

        public ItinerairePainter(List<GeoPosition> itineraire) {
            this.itineraire = itineraire;
        }

        @Override
        public void paint(Graphics2D g, JXMapViewer map, int w, int h) {
            g = (Graphics2D) g.create();
            Rectangle rect = map.getViewportBounds();
            g.translate(-rect.x, -rect.y);
            g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);
            g.setColor(Color.RED);
            g.setStroke(new BasicStroke(3));
            drawRoute(g, map, itineraire);
            g.dispose();
        }

        private void drawRoute(Graphics2D g, JXMapViewer map, List<GeoPosition> positions) {

            Point2D p1 = map.getTileFactory().geoToPixel(positions.get(0), map.getZoom());
            Point2D p2 = map.getTileFactory().geoToPixel(positions.get(positions.size() - 1), map.getZoom());

            int lastX = 0;
            int lastY = 0;
            boolean first = true;

            for (GeoPosition gp : positions) {

                Point2D pt = map.getTileFactory().geoToPixel(gp, map.getZoom());

                if (first) {
                    first = false;
                } else {
                    if (lastX == p2.getX() && lastY == p2.getY() && pt.getX() == p1.getX() && pt.getY() == p1.getY()) {
                        break;
                    }
                    g.drawLine(lastX, lastY, (int) pt.getX(), (int) pt.getY());
                }
                lastX = (int) pt.getX();
                lastY = (int) pt.getY();
            }
        }
    }
}

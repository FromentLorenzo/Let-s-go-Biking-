package map.utilities;

import org.jxmapviewer.JXMapViewer;
import org.jxmapviewer.painter.CompoundPainter;
import org.jxmapviewer.viewer.*;
import org.jxmapviewer.painter.Painter;

import java.awt.*;
import java.awt.geom.Point2D;
import java.util.*;
import java.util.List;

public class Itineraire {
    private List<GeoPosition> points;
    private Color color = Color.RED;

    // Constructeur
    public Itineraire(List<GeoPosition> points) {
        // Initialise la liste de points de l'itinéraire
        this.points = new ArrayList<>(points);
    }

    // Méthode d'accès à la liste de points
    public List<GeoPosition> getpoints() {
        return points;
    }

    // Méthode d'accès à la couleur de l'itinéraire
    public Color getColor() {
        return this.color;
    }

    // Méthode de modification de la couleur de l'itinéraire
    public void setColor(Color color) {
        this.color = color;
    }

    // Méthode statique pour afficher un itinéraire sur la carte
    static public void displayItineraire(JXMapViewer mapViewer, Itineraire itineraire, int indexStation1, int indexStation2) {
        // Crée un ensemble de waypoints pour les points spécifiques de l'itinéraire
        Set<Waypoint> waypoints = new HashSet<>();
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(0)));
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(indexStation1)));
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(indexStation2)));
        waypoints.add(new DefaultWaypoint(itineraire.getpoints().get(itineraire.getpoints().size() - 1)));

        // Crée un WaypointPainter pour afficher les waypoints
        WaypointPainter<Waypoint> waypointPainter = new WaypointPainter<>();
        waypointPainter.setWaypoints(waypoints);

        // Crée un ItinerairePainter pour afficher l'itinéraire
        Painter<JXMapViewer> itinerairePainter = new ItinerairePainter(itineraire.getpoints(), itineraire.getColor());

        // Crée une liste de peintres pour être utilisée comme un seul peintre composé
        List<Painter<JXMapViewer>> painters = new ArrayList<>();
        painters.add(itinerairePainter);
        painters.add(waypointPainter);

        // Crée un CompoundPainter pour combiner les peintres
        CompoundPainter<JXMapViewer> painter = new CompoundPainter<>(painters);

        // Définit le peintre composé comme un overlay sur la carte
        mapViewer.setOverlayPainter(painter);
    }

    // Classe interne privée pour peindre l'itinéraire
    private static class ItinerairePainter implements Painter<JXMapViewer> {
        private final List<GeoPosition> itineraire;
        private Color color;

        // Constructeur
        public ItinerairePainter(List<GeoPosition> itineraire, Color color) {
            // Initialise les points et la couleur de l'itinéraire
            this.itineraire = itineraire;
            this.color = color;
        }

        // Méthode pour peindre l'itinéraire sur la carte
        @Override
        public void paint(Graphics2D g, JXMapViewer map, int w, int h) {
            // Crée une copie du contexte graphique
            g = (Graphics2D) g.create();

            // Translate le contexte graphique pour correspondre à la vue de la carte
            Rectangle rect = map.getViewportBounds();
            g.translate(-rect.x, -rect.y);

            // Active le lissage graphique pour un rendu plus esthétique
            g.setRenderingHint(RenderingHints.KEY_ANTIALIASING, RenderingHints.VALUE_ANTIALIAS_ON);

            // Configure la couleur et l'épaisseur du trait
            g.setColor(this.color);
            g.setStroke(new BasicStroke(3));

            // Dessine l'itinéraire
            drawRoute(g, map, itineraire);

            // Libère le contexte graphique
            g.dispose();
        }

        // Méthode pour dessiner la ligne représentant l'itinéraire
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

import com.soap.ws.client.generated.GetRoute;
import com.soap.ws.client.generated.Service1;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;
import java.util.ArrayList;
import java.util.Arrays;
import java.util.HashSet;
import java.util.List;

import map.utilities.*;
import org.jxmapviewer.JXMapKit;
import org.jxmapviewer.JXMapViewer;
import org.jxmapviewer.OSMTileFactoryInfo;
import org.jxmapviewer.viewer.*;

import static map.utilities.Itineraire.displayItineraire;

public class Main extends JFrame {

    private JXMapViewer mapViewer;
    private JTextField departField;
    private JTextField arriveeField;
    private JTextArea resultArea;

    public Main() {
        initializeUI();
    }

    private void initializeUI() {
        // Configuration de la JFrame
        setTitle("Itinéraire Finder");
        setDefaultCloseOperation(JFrame.EXIT_ON_CLOSE);
        setExtendedState(JFrame.MAXIMIZED_BOTH);  // Met en plein écran
        setLocationRelativeTo(null);

        // Création des composants
        JLabel departLabel = new JLabel("Adresse de départ:");
        departField = new JTextField(13); // Ajusté la taille de la colonne

        JLabel arriveeLabel = new JLabel("Adresse d'arrivée:");
        arriveeField = new JTextField(13); // Ajusté la taille de la colonne

        JButton findRouteButton = new JButton("Trouver l'itinéraire");
        JButton callMQButton = new JButton("Appeler MQ");

        // Ajout d'un gestionnaire d'événements au bouton "Trouver l'itinéraire"
        findRouteButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                onFindRouteButtonClicked();
            }
        });

        // Ajout d'un gestionnaire d'événements au bouton "Appeler MQ"
        callMQButton.addActionListener(new ActionListener() {
            @Override
            public void actionPerformed(ActionEvent e) {
                callMQButtonClicked();
            }
        });

        // Configuration du JTextArea pour afficher les résultats
        resultArea = new JTextArea();
        resultArea.setEditable(false);
        JScrollPane scrollPane = new JScrollPane(resultArea);

        // Configuration du layout avec deux panneaux
        JPanel leftPanel = new JPanel(new GridBagLayout());
        JPanel rightPanel = new JPanel(new BorderLayout());
        JPanel mapPanel = new JPanel();
        JSplitPane splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, leftPanel, rightPanel);
        JSplitPane splitPane2 = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, splitPane, mapPanel);

        // Ajout des composants à gauche avec un layout GridBagLayout
        GridBagConstraints gbc = new GridBagConstraints();
        gbc.insets = new Insets(5, 5, 5, 5);
        gbc.anchor = GridBagConstraints.WEST;
        gbc.gridx = 0;
        gbc.gridy = 0;
        leftPanel.add(departLabel, gbc);

        gbc.gridy++;
        leftPanel.add(departField, gbc);

        gbc.gridy++;
        leftPanel.add(arriveeLabel, gbc);

        gbc.gridy++;
        leftPanel.add(arriveeField, gbc);

        gbc.gridy++;
        leftPanel.add(findRouteButton, gbc);

        gbc.gridy++;
        leftPanel.add(callMQButton, gbc);

        // Ajout du JSplitPane à la JFrame
        add(splitPane2);

        // Ajout du JScrollPane avec JTextArea à droite
        rightPanel.add(scrollPane, BorderLayout.CENTER);

        // Définir une taille initiale pour la colonne de la JTextArea
        splitPane.setDividerLocation(0.75);
        splitPane2.setDividerLocation(0.25);

        //map viewer
        mapViewer = new JXMapKit().getMainMap();
        mapViewer.setVisible(false);

        // Create a TileFactoryInfo for OpenStreetMap
        TileFactoryInfo info = new OSMTileFactoryInfo();
        DefaultTileFactory tileFactory = new DefaultTileFactory(info);
        mapViewer.setTileFactory(tileFactory);

        // Use 8 threads in parallel to load the tiles
        tileFactory.setThreadPoolSize(8);


        mapPanel.setLayout(new BorderLayout());
        mapPanel.add(mapViewer, BorderLayout.CENTER);

        // Affichage de la JFrame
        setVisible(true);
    }

    private void onFindRouteButtonClicked() {
        // Récupération des adresses saisies
        String depart = departField.getText();
        String arrivee = arriveeField.getText();

        // Appel à la méthode pour obtenir l'itinéraire
        Service1 service1 = new Service1();
        GetRoute getRoute = new GetRoute();
        String route = service1.getBasicHttpBindingIService1().getRoute(depart, arrivee);

        mapViewer.setVisible(true);

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

        try {
            mapViewer.zoomToBestFit(new HashSet<>(allInstructions), 0.5);
        } catch (Exception e) {
            mapViewer.setZoom(10);
        }

        Itineraire itineraire = new Itineraire(allInstructions);
        displayItineraire(mapViewer, itineraire, walk1.size(), walk1.size()+bike.size());


        // Ajout des instructions dans le JTextArea sans effacer le texte existant
        resultArea.append("\nAppuyez sur le bouton \"Appeler MQ\" pour obtenir l'itinéraire");
    }

    private void callMQButtonClicked() {
        // Appeler la méthode statique callMQ de la classe MQ
        String message = MQ.callMQ();

        // Ajouter le message dans le JTextArea sans effacer le texte existant
        resultArea.append("\n" + message);
    }

    public static void main(String[] args) {
        SwingUtilities.invokeLater(new Runnable() {
            @Override
            public void run() {
                new Main();
            }
        });
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

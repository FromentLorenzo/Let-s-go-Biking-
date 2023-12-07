import com.soap.ws.client.generated.GetRoute;
import com.soap.ws.client.generated.Service1;

import javax.swing.*;
import java.awt.*;
import java.awt.event.ActionEvent;
import java.awt.event.ActionListener;

public class Main extends JFrame {

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
        JSplitPane splitPane = new JSplitPane(JSplitPane.HORIZONTAL_SPLIT, leftPanel, rightPanel);

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
        add(splitPane);

        // Ajout du JScrollPane avec JTextArea à droite
        rightPanel.add(scrollPane, BorderLayout.CENTER);

        // Définir une taille initiale pour la colonne de la JTextArea
        splitPane.setDividerLocation(0.75);

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
}

import java.io.BufferedReader;
import java.io.InputStreamReader;
import java.net.HttpURLConnection;
import java.net.URL;

public class BikeRouteClient {

    public static void main(String[] args) {
        // Demander à l'utilisateur les points de départ et d'arrivée
        String origin = getUserInput("Entrez le point de départ : ");
        String destination = getUserInput("Entrez le point d'arrivée : ");

        // Appeler le serveur de routage
        String itinerary = getBikeItinerary(origin, destination);

        // Afficher l'itinéraire
        System.out.println("Itinéraire :");
        System.out.println(itinerary);
    }

    private static String getUserInput(String message) {
        System.out.print(message);
        BufferedReader reader = new BufferedReader(new InputStreamReader(System.in));
        try {
            return reader.readLine();
        } catch (Exception e) {
            e.printStackTrace();
            return null;
        }
    }

    private static String getBikeItinerary(String origin, String destination) {
        try {
            // Remplacez l'URL par l'adresse réelle de votre serveur de routage
            String apiUrl = "http://localhost:8080/getItinerary?origin=" + origin + "&destination=" + destination;
            URL url = new URL(apiUrl);

            HttpURLConnection connection = (HttpURLConnection) url.openConnection();
            connection.setRequestMethod("GET");

            int responseCode = connection.getResponseCode();
            if (responseCode == HttpURLConnection.HTTP_OK) {
                BufferedReader reader = new BufferedReader(new InputStreamReader(connection.getInputStream()));
                String line;
                StringBuilder response = new StringBuilder();

                while ((line = reader.readLine()) != null) {
                    response.append(line);
                }

                reader.close();
                return response.toString();
            } else {
                System.out.println("Erreur lors de la récupération de l'itinéraire. Code de réponse : " + responseCode);
            }
        } catch (Exception e) {
            e.printStackTrace();
        }

        return "Erreur lors de la récupération de l'itinéraire.";
    }
}
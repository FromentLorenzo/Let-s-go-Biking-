import com.soap.ws.client.generated.GetRoute;
import com.soap.ws.client.generated.Service1;

import java.util.Scanner;

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
        System.out.println(route);

        // Place du Général-de-Gaulle, Rouen
        // Avenue Martyrs de la Résistance, Rouen
        // rue paul bert, lyon
        // rue des rancy, lyon
    }
}
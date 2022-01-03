class CustomCoffeeMaker extends HTMLElement {
    connectedCallback() {
        this.innerHTML = `<div id="app">
                            <v-app>
                                <v-main>
                                    <v-container>
                                        <v-row>
                                            <v-col cols="12"
                                                   md="4">

                                                <v-card elevation="2"
                                                        outlined>
                                                    <v-skeleton-loader v-bind="attrs" type="article">
                                                    </v-skeleton-loader>
                                                </v-card>
                                            </v-col>
                                        </v-row>
                                    </v-container>

                                </v-main>
                            </v-app>
                        </div>
                        `
        new Vue({
            el: '#app',
            vuetify: new Vuetify(),
        })
        this.GetContent(this);
    }

    GetContent(el) {
        $.getJSON("https://brewmasterweb.azurewebsites.net/api/coffeemakers", function (data) {
            data = data[0];
            var template = `<div id="app">
                            <v-app>
                                <v-main>
                                    <v-container>
                                        <v-row>
                                            <v-col cols="12"
                                                   md="4">
  <v-card
    class="mx-auto"
    max-width="344"
    outlined
  >
    <v-list-item three-line>
      <v-list-item-content>
        <div class="text-overline mb-4">
          Coffee Maker
        </div>
        <v-list-item-title class="text-h5 mb-1">
          ${data.name}
        </v-list-item-title>
        <v-list-item-subtitle>Last Brewed: ${data.lastStartDateTime}</v-list-item-subtitle>
      </v-list-item-content>
    </v-list-item>

    <v-card-actions>
      <v-btn
        outlined
        rounded
        text
      >
        Subscribe to Coffee Maker
      </v-btn>
    </v-card-actions>
  </v-card>
  </v-col>
                                        </v-row>
                                    </v-container>
    </v-main>
                            </v-app>
                        </div>`;

            el.innerHTML = template;
            new Vue({
                el: '#app',
                vuetify: new Vuetify(),
            })
        });
    }
}

window.customElements.define('custom-coffeemaker', CustomCoffeeMaker);

const baseUrl = "http://localhost:14335";

class CustomCoffeeMaker extends HTMLElement {

    mainTemplate = `
<div id="app">
    <v-app>
        <v-main>
            <v-container v-if="coffeeMakers == null">
                <v-row >
                    <v-col cols="12" md="4">
                        <v-card elevation="2" outlined>
                            <v-skeleton-loader v-bind="attrs" type="article">
                            </v-skeleton-loader>
                        </v-card>
                    </v-col>
                </v-row>
            </v-container>
            <v-container v-else>
                <v-row>
                    <v-col cols="12" md="4"  v-for="coffeeMaker in coffeeMakers" :key="coffeeMaker.id">
                        <v-card class="mx-auto" max-width="344" outlined >
                            <v-row>
                                <v-col cols="9">
                                    <div class="text-overline mb-4">
                                        Coffee Maker
                                    </div>
                                </v-col>
                                <v-col cols="2">
                                    <v-btn rounded icon color="grey">
                                        <i class="fa fa-pencil"></i>
                                    </v-btn>
                                </v-cols>
                            </v-row>
                            <v-list-item three-line>
                                <v-list-item-content>
                                    <v-list-item-title class="text-h5 mb-1">
                                        {{coffeeMaker.name}}
                                    </v-list-item-title>
                                    <v-list-item-subtitle>
                                        Last Brewed:  {{coffeeMaker.lastCompeteDateTime}}
                                    </v-list-item-subtitle>
                                </v-list-item-content>
                            </v-list-item>
                            <v-card-actions>
                                <v-btn outlined rounded text v-if="coffeeMaker.isSubscribed"
                                    :loading="itemLoading == coffeeMaker.id"
                                    :disabled="itemLoading == coffeeMaker.id"
                                    v-on:click="unsubscribe(coffeeMaker, personToken)">
                                    <i class="fa fa-check green--text"></i> Subscribed
                                </v-btn>
                                <v-btn outlined rounded text v-else
                                    :loading="itemLoading == coffeeMaker.id"
                                    :disabled="itemLoading == coffeeMaker.id"
                                    v-on:click="subscribe(coffeeMaker,personToken)">
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


    connectedCallback() {
        this.innerHTML = this.mainTemplate;
        var vue = new Vue({
            el: '#app',
            vuetify: new Vuetify(),
            data: {
                coffeeMakers: null,
                personToken: this.getAttribute("data-person-token"),
                itemLoading: undefined
            },
            methods: {
                subscribe: function (coffeeMaker, personToken) {
                    this.itemLoading = coffeeMaker.id;

                    $.post(baseUrl + "/api/subscribe", { "coffeeMakerId": coffeeMaker.id, "personToken": personToken })
                        .done(function (data) {
                            if (vue.itemLoading === coffeeMaker.id) {
                                vue.itemLoading = undefined;
                            }
                            coffeeMaker.isSubscribed = true;
                        });
                },
                unsubscribe: function (coffeeMaker, personToken) {
                    this.itemLoading = coffeeMaker.id;

                    $.post(baseUrl + "/api/unsubscribe", { "coffeeMakerId": coffeeMaker.id, "personToken": personToken })
                        .done(function (data) {
                            if (vue.itemLoading === coffeeMaker.id) {
                                vue.itemLoading = undefined;
                            }
                            coffeeMaker.isSubscribed = false;
                        });
                }
            }
        });
        this.GetContent(vue);
    }

    GetContent(vue) {
        $.getJSON(baseUrl + "/api/coffeemakersforperson?persontoken=" + vue.personToken, function (data) {
            vue.coffeeMakers = data;
        });
    }
}

window.customElements.define('custom-coffeemaker', CustomCoffeeMaker);

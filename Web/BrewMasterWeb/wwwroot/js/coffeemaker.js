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
            <v-container v-else flex>
                <v-row>
                    <v-col cols="12" md="4"  v-for="coffeeMaker in coffeeMakers" :key="coffeeMaker.id">
                        <v-card class="mx-auto" outlined >
                            <v-row>
                                <v-col cols="8">
                                    <div class="text-overline mb-4 ml-2">
                                        Coffee Maker <span v-if="coffeeMaker.isActive!=true">- NOT PUBLICLY VIEWABLE</span>
                                    </div>
                                </v-col>
                                <v-col cols="4" class="text-right">
                                    <v-switch v-model="coffeeMaker.isActive" color="green" @change="toggleCoffeeMaker(coffeeMaker, personToken)"
                                        hide-details class="float-right mt-1"></v-switch>
                                    <v-btn rounded icon color="grey" v-on:click="edit(coffeeMaker)" class="float-right" v-if="isAdmin==true">
                                        <i class="fa fa-pencil"></i>
                                    </v-btn>
                                </v-cols>
                            </v-row class="ml-4">
                            <v-list-item three-line>
                                <v-list-item-content>
                                    <v-text-field v-if="itemEditing == coffeeMaker.id" v-model="coffeeMaker.name"
                                      label="Coffee Maker Name" :rules="required" hide-details="auto" class="text-h5"
                                      v-on:change="updateName(coffeeMaker, personToken)" >
                                    </v-text-field>
                                    <v-list-item-title class="text-h5 mb-1" v-else>
                                        {{coffeeMaker.name}}
                                    </v-list-item-title>
                                    <v-list-item-subtitle>
                                        Last brewed  {{coffeeMaker.lastBrewTime}} ago
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
                itemLoading: undefined,
                itemEditing: undefined,
                isAdmin: false,
                required: [
                    value => !!value || 'Required.',
                    value => (value && value.length >= 3) || 'Min 3 characters',
                ]
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
                },
                edit: function (coffeeMaker) {
                    if (this.itemEditing === coffeeMaker.id) {
                        this.itemEditing = undefined;
                    }
                    else {
                        this.itemEditing = coffeeMaker.id;
                    }
                },
                updateName: function (coffeeMaker,personToken) {
                    var payload = {
                        "id": coffeeMaker.id,
                        "name": coffeeMaker.name,
                        "personToken":personToken
                    };

                    $.post("/api/updatename", payload);
                    this.itemEditing = undefined;
                },
                toggleCoffeeMaker: function (coffeeMaker, personToken) {
                    var payload = {
                        "id": coffeeMaker.id,
                        "isActive": coffeeMaker.isActive,
                        "personToken": personToken
                    };

                    $.post("/api/toggleCoffeeMaker", payload);
                }
            }
        });
        this.GetContent(vue);
    }

    GetContent(vue) {
        $.getJSON(baseUrl + "/api/coffeemakersforperson?persontoken=" + vue.personToken, function (data) {
            vue.coffeeMakers = data.coffeeMakers;
            vue.isAdmin = data.isAdmin
        });
    }
}

window.customElements.define('custom-coffeemaker', CustomCoffeeMaker);

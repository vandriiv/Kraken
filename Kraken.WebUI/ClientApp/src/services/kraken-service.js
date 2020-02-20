import axios from 'axios';
export default class KrakenService {

    computeNormalModes = (acousticData) => {
        return axios.post("/kraken/computeNormalModes", acousticData)
            .then(response => {
                return response.data;
            })
            .catch(ex => {
                throw { message: 'An error has been ocurred', error: ex };
            });
    }

}
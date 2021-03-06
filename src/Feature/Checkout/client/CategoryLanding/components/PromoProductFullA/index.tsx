//    Copyright 2019 EPAM Systems, Inc.
// 
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
// 
//      http://www.apache.org/licenses/LICENSE-2.0
// 
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.

import * as React from 'react';

import { Text, withExperienceEditorChromes } from '@sitecore-jss/sitecore-jss-react';
import { PromoProductFullAControlProps, PromoProductFullAControlState } from './models';

import * as Jss from 'Foundation/ReactJss/client';

import './styles.scss';

class PromoProductFullAControl extends Jss.SafePureComponent<PromoProductFullAControlProps, PromoProductFullAControlState> {
  public safeRender() {
    return (
        <figure className="product-promo-full-a">
            <a href="#">
              <img
                alt="Image Alt Text"
                src="https://placeholdit.imgix.net/~text?txtsize=20&txt=watch&w=1280&h=629"
              />
              <figcaption>
                <h2 className="product-promo-full-a-text">
                  <span className="text-style1" />
                </h2>
                <Text
                  field={{ value: 'Add to Cart +' }}
                  tag="button"
                  className="btn-product-promo-full-a"
                />
              </figcaption>
            </a>
        </figure>
    );
  }
}

export const PromoProductFullA = withExperienceEditorChromes(PromoProductFullAControl);

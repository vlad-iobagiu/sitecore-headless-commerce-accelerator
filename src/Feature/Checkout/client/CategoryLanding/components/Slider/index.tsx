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
import { Carousel } from 'Foundation/UI/client';
import { SliderControlProps, SliderControlState } from './models';

import * as Jss from 'Foundation/ReactJss/client';

import './styles.scss';

class SliderControl extends Jss.SafePureComponent<SliderControlProps, SliderControlState> {
  public safeRender() {
    const images: string[] = new Array(10)
      .fill(undefined)
      .map(() => 'https://placeholdit.imgix.net/~text?txtsize=20&txt=watch&w=74&h=77');

    return (
      <div className="slider sub-categories">
        <div className="slider-header">
          <div className="rainbow-title">
            <Text
              field={{ value: 'Digital Training' }}
              tag="h2"
              className="title"
            />
            <div className="rainbow-bar rainbow-bar-4" />
          </div>
        </div>

        <Carousel
          className="gallery-thumbs"
          buttonPreviousText={<i className="fa fa-angle-left" />}
          buttonNextText={<i className="fa fa-angle-right" />}
          options={{
            breakpoints: {
              480: {
                slidesPerView: 2,
                spaceBetween: 25
              },
              768: {
                slidesPerView: 3,
                spaceBetween: 25
              },
              1310: {
                slidesPerView: 4,
                spaceBetween: 25
              },
            },
            slidesPerView: 6,
            spaceBetween: 25,
          }}
        >
          {images &&
            images.map((item, index) => (
              <figure key={index} className="swiper-slide item">
                <div className="image-wrap">
                  <img src={item} alt={index.toString()}/>
                </div>
                <figcaption className="item-cap">
                  <h2 className="item-title">
                    <a href="#" title="tooltip">
                      Item title
                    </a>
                  </h2>
                </figcaption>
              </figure>
            ))}
        </Carousel>
      </div>
    );
  }
}

export const Slider = withExperienceEditorChromes(SliderControl);

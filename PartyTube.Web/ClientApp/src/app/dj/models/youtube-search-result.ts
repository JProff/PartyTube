import { VideoItem } from './video-item';

export class YoutubeSearchResult {
  total: number;
  nextPageToken: string;
  videos: VideoItem[];
}
